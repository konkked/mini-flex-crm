import React, { useState, useEffect } from "react";
import { Table, Button, Form } from "react-bootstrap";
import { toast } from "react-toastify";
import { PencilSquare, ChevronLeft, ChevronRight, Trash, Floppy, X } from "react-bootstrap-icons"; // Bootstrap Icons
import "./paginated-list.css"; // Custom styles for button positioning
import { useQueryState, useGenQueryState } from "hooks/useQueryState";

interface PaginatedListProps {
  fetch: (offset?: number, limit?: number) => Promise<any[]>;
  defaultPageSize?: number;
  editItem?: (item: any) => Promise<void>;
  columns: {
    key: string;
    label: string;
    editable?: boolean | ((item: any) => boolean);
    linkTo?: (id: number) => string;
    convert?: (value: any) => string;
    visible?: boolean | ((item: any) => boolean);
    options?: { label: string; value: string }[];
  }[];
}

const PaginatedList = ({ defaultPageSize, fetch: fetchNext, editItem, columns }: PaginatedListProps) => {
  const [offset, setOffset] = useGenQueryState<number>("offset", i=>Number.parseInt(i ?? "0"), JSON.stringify);
  const [count, setCount]= useGenQueryState<number>("count", i=>Number.parseInt(i ?? `${defaultPageSize}`), JSON.stringify);
  const [editingIndex, setEditingIndex] = useState<number | null>(null);
  const [editingItem, setEditingItem] = useState<any | null>(null);
  const [items, setItems] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadData = async () => {
      setLoading(true);
      try {
        const response = await fetchNext(offset ?? 0, count ?? 50);
        setEditingItem(null);
        setItems(response);
      } catch (error) {
        toast.error("Error fetching data");
      }
      setLoading(false);
    };
    loadData();
  }, [offset, count, fetchNext]);

  if (loading) {
    return <div>Loading...</div>;
  }

  interface RowProps {
    editingIndex: number | null;
    index: number;
    item: any;
  }

  const RowItem: React.FC<RowProps> = ({ editingIndex, index, item }) => {
    if (
      columns.some(
        (col) =>
          (col.visible !== undefined && col.visible === false) ||
          (typeof col.visible === "function" && !col.visible(item))
      )
    ) {
      return <></>;
    }
    if (editingIndex === null || editingIndex !== index) {
      return columns.map((col) => (
        <td key={col.key}>
          {col.convert ? (
            col.convert(item[col.key])
          ) : col.linkTo ? (
            <a href={col.linkTo(item.id)}>{item[col.key]}</a>
          ) : (
            item[col.key]
          )}
        </td>
      ));
    }

    function invokeIfEditableFunction(col: any, item: any) {
      if (typeof col.editable === "function") {
        return col.editable(item);
      }
      return item;
    }
    return columns.map((col) =>
      col.editable === false || !invokeIfEditableFunction(col, item) ? (
      <td key={col.key}>
        {col.convert ? col.convert(editingItem[col.key]) : editingItem[col.key]}
      </td>
      ) : (
      <td key={col.key}>
        {typeof editingItem[col.key] === "number" ? (
        <Form.Control
          type="number"
          value={editingItem[col.key]}
          onChange={(e) =>
          setEditingItem({
            ...editingItem,
            [col.key]: parseFloat(e.target.value),
          })
          }
        />
        ) : typeof editingItem[col.key] === "boolean" ? (
        <Form.Control
          as="select"
          value={editingItem[col.key] ? "true" : "false"}
          onChange={(e) =>
          setEditingItem({
            ...editingItem,
            [col.key]: e.target.value === "true",
          })
          }
        >
          <option value="true">True</option>
          <option value="false">False</option>
        </Form.Control>
        ) : col.options ? (
        <Form.Control
          as="select"
          value={editingItem[col.key]}
          onChange={(e) =>
          setEditingItem({
            ...editingItem,
            [col.key]: e.target.value,
          })
          }
        >
          {col.options.map((option) => (
          <option key={option.label} value={option.value}>
            {option.label}
          </option>
          ))}
        </Form.Control>
        ) : (
        <Form.Control
          type="text"
          value={editingItem[col.key]}
          onChange={(e) =>
          setEditingItem({
            ...editingItem,
            [col.key]: e.target.value,
          })
          }
        />
        )}
      </td>
      )
    );
  };

  const EditingOptionsRowPart: React.FC<RowProps> = ({ editingIndex, index, item }) => {
    if (!columns.some((col) => col.editable === true || typeof col.editable === "function")) {
      return <></>;
    }
    return (
      <td>
        {(editingIndex == null || editingIndex !== index) && (
          <Button
            variant="outline-primary"
            className="me-2"
            onClick={() => {
              setEditingItem({ ...item });
              setEditingIndex(index);
            }}
          >
            <PencilSquare size={18} /> Edit
          </Button>
        )}
        {editingIndex === index && typeof editItem == "function" && (
          <Button
            variant="outline-primary"
            className="me-2"
            onClick={() => {
              editItem(item);
              items[index] = editingItem;
              setItems([...items]);
              setEditingIndex(null);
              setEditingItem(null);
            }}
          >
            <Floppy size={18} /> Save
          </Button>
        )}
        {editingIndex === index && typeof editItem == "function" && (
          <Button
            variant="outline-secondary"
            onClick={() => {
              setEditingIndex(null);
              setEditingItem(null);
            }}
          >
            <X size={18} /> Cancel
          </Button>
        )}
      </td>
    );
  };

  return (
    <div>
      <Table striped bordered hover>
        <thead>
          <tr>
            {columns.map((col) => (
              <th key={col.key}>{col.label}</th>
            ))}
            {columns.some((col) => col.editable === true || typeof col.editable === "function") && <th>Actions</th>}
          </tr>
        </thead>
        <tbody>
          {items.map((item, index) => (
            <tr key={index}>
              <RowItem editingIndex={editingIndex} index={index} item={item} />
              <EditingOptionsRowPart editingIndex={editingIndex} index={index} item={item} />
            </tr>
          ))}
        </tbody>
      </Table>
      <div className="pagination-container">
        <div className="d-flex justify-content-center align-items-center w-100">
          <div className="col-2 text-center">
            {(offset ?? 0) > 0 && (
              <Button
                variant="primary"
                onClick={() => {
                  setOffset(Math.max(0, (offset ?? 50) - (count ?? 50)));
                }}
                className="pagination-button left"
              >
                <ChevronLeft size={20} />
              </Button>
            )}
          </div>
          <div className="col-1"></div>
          <div className="col-3 text-center">
            <Form.Control
              as="select"
              value={count ?? 50}
              onChange={(e) => {
                setCount(parseInt(e.target.value));
              }}
            >
              {[10, 20, 50, 100, 200].map((value) => (
                <option key={value} value={value}>
                  {value}
                </option>
              ))}
            </Form.Control>
          </div>
          <div className="col-1"></div>
          <div className="col-2 text-center">
            {items.length === count && (
              <Button
                variant="primary"
                onClick={() => {
                  setOffset((offset ?? 0) + count);
                }}
                className="pagination-button right"
              >
                <ChevronRight size={20} />
              </Button>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default PaginatedList;
