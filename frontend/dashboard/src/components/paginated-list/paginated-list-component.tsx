import React, { useState, useEffect } from "react";
import { Table, Button, Form, Container, Row, Col } from "react-bootstrap";
import { toast } from "react-toastify";
import { PencilSquare, ChevronLeft, ChevronRight, Trash, Floppy, X } from "react-bootstrap-icons"; // Bootstrap Icons
import "./paginated-list-component.css"; // Custom styles for button positioning
import { useGenQueryState } from "hooks/useQueryState";

export interface ColumnDefinition {
  key: string;
  label: string;
  editable?: boolean | ((item: any) => boolean);
  linkTo?: (id: number) => string;
  convert?: (value: any) => string;
  visible?: boolean | ((item: any) => boolean);
  options?: { value: string; label: string }[];
  render?: (item: any) => React.ReactNode;
  width?: string;
}

interface PaginatedListProps {
  fetch: (offset?: number, limit?: number) => Promise<any[]>;
  defaultPageSize?: number;
  editItem?: (item: any) => Promise<void>;
  deleteItem?: (item: any) => Promise<void>;
  columns: ColumnDefinition[];
}

const PaginatedList = ({ defaultPageSize, fetch, editItem, columns }: PaginatedListProps) => {
  const [offset, setOffset] = useGenQueryState<number>("offset", i=>Number.parseInt(i ?? "0"), JSON.stringify);
  const [count, setCount]= useGenQueryState<number>("count", i=>Number.parseInt(i ?? `${defaultPageSize}`) || 50, JSON.stringify);
  const [editingIndex, setEditingIndex] = useState<number | null>(null);
  const [editingItem, setEditingItem] = useState<any | null>(null);
  const [items, setItems] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadData = async () => {
      setLoading(true);
      try {
        const response = await fetch(offset ?? 0, count ?? 50);
        setEditingItem(null);
        setItems(response);
        console.log('response:' , response);
      } catch (error) {
        toast.error("Error fetching data");
      }
      setLoading(false);
    };
    loadData();
  }, [offset, count, fetch]);

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
          {col.render ? (
            col.render(item)
          ) : col.convert ? (
            col.convert(item[col.key])
          ) : col.linkTo ? (
            <a href={col.linkTo(item.id)}>{item[col.key]}</a>
          ) : (
             (function(){/*console.log(`adding column ${col.key} with value ${item[col.key]}`);*/return `${item[col.key]}`;})()
          )}
        </td>
      ));
    }

    function isColumnEditable(col: any, item: any) {
      if (!editItem && typeof editItem !== "function") {
        console.log('edit item is not a function, column not editable.');
        return false;
      }
      if (typeof col.editable === "function") {
        console.log('col.editable(item) 1:', col.editable(item));
        return col.editable(item);
      }
      if (col.editable === true) {
        console.log('col.editable(item) 2:', true);
        return true;
      }

      console.log('col.editable(item) 3:', !!col.editable);
      return !!col.editable;
    }
    return columns.map((col) =>
      editingIndex !== index || !isColumnEditable(col, item) ? (
        <td key={col.key}>
          {col.render ? (
            col.render(item)
          ) : col.convert ? (
            col.convert(editingItem[col.key])
          ) : editingItem[col.key]}
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
                  [col.key]: parseFloat(isColumnEditable(col, e.target.value)),
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
                  [col.key]: isColumnEditable(col, e.target.value) === "true",
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
                  [col.key]: isColumnEditable(col, e.target.value),
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
                  [col.key]: isColumnEditable(col, e.target.value),
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

  const renderCell = (item: any, column: ColumnDefinition) => {
    if (column.render) {
      return column.render(item);
    }
    
    const value = item[column.key];
    if (column.convert) {
      return column.convert(value);
    }
    return value;
  };

  return (
    <Container fluid>
      <Row>
      <Col md={{ span: 8, offset: 2 }}>  
      <Table striped bordered hover>
        <thead>
          <tr>
            {columns
              .filter((column) => column.visible === undefined || column.visible === true || (typeof column.visible === 'function' && column.visible(items[0])))
              .map((column) => (
                <th key={column.key} style={column.width ? { width: column.width } : undefined}>{column.label}</th>
              ))}
            {columns.some((col) => col.editable === true || typeof col.editable === "function") 
              && typeof editItem == "function" 
              && <th>Actions</th>}
          </tr>
        </thead>
        <tbody>
          {items.map((item) => (
            <tr key={item.id}>
              {columns
                .filter((column) => column.visible === undefined || column.visible === true || (typeof column.visible === 'function' && column.visible(item)))
                .map((column) => (
                  <td key={`${item.id}-${column.key}`} style={column.width ? { width: column.width } : undefined}>
                    {renderCell(item, column)}
                  </td>
                ))}
              {columns.some((col) => col.editable === true || typeof col.editable === "function")
                && typeof editItem == "function" 
                && <EditingOptionsRowPart 
                    editingIndex={editingIndex} 
                    index={items.indexOf(item)} 
                    item={item} />}
            </tr>
          ))}
        </tbody>
      </Table>
      {((offset ?? 0) > 0 || items.length === count) && <div className="pagination-container">
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
      </div>}
      </Col>
      </Row>
    </Container>
  );
};

export default PaginatedList;
