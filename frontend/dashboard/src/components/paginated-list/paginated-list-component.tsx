import React, { useState, useEffect } from "react";
import { Table, Button, Form } from "react-bootstrap";
import { toast } from "react-toastify";
import { useQueryState } from "../../hooks/useQueryState";
import { PencilSquare, Trash, ChevronLeft, ChevronRight, Floppy, X } from "react-bootstrap-icons"; // Bootstrap Icons
import "./paginated-list.css"; // Custom styles for button positioning

interface Page{ items: any[]; next?: string; prev?: string}

interface PaginatedListProps {
  initialItems?: any[] | null;
  nextItems?: (token?: string, search?: string) => Promise<Page>;
  prevItems?: (token?: string, search?: string) => Promise<Page>;
  editItem?: (item: any) => Promise<void>;
  deleteItem?: (item: any) => Promise<void>;
  columns: {
    key: string;
    label: string;
    editable?: boolean | ((item: any) => boolean);
    linkTo?: (id: number) => string;
    convert?: (value: any) => string;
    visible?: boolean | ((item: any) => boolean);
    options?: { label: string; value: string}[];
  }[];
}

const PaginatedList = ({ initialItems, nextItems, prevItems, editItem, deleteItem, columns } : PaginatedListProps) => {
  const [search] = useQueryState("search");
  const [prev, setprev] = useQueryState("prev");
  const [next, setnext] = useQueryState("next");
  const [editingIndex, setEditingIndex] = useState<number | null>(null);
  const [editingItem, setEditingItem] = useState<any | null>(null);
  let [items, setItems] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);

    useEffect(() => {
      if(nextItems) {
          const loadData = async () => {
            setLoading(true);
            try {
              const response = await nextItems(next || undefined, search);
              setEditingItem(null);
              setItems(response.items);
              setnext(response.next || null);
              setprev(response.prev || null);
            } catch (error) {
              toast.error("Error fetching data");
            }
            setLoading(false);
          };
          if (next) {
            loadData();
          }
      } else if (prevItems) {
        const loadData = async () => {
          setLoading(true);
          try {
            const response = await prevItems(prev || undefined, search);
            setEditingItem(null);
            setItems(response.items);
            setnext(response.next || null);
            setprev(response.prev || null);
          } catch (error) {
            toast.error("Error fetching data");
          }
          setLoading(false);
        };
        if (prev) {
          loadData();
        }
      }
    }, [next, search]);

  if(initialItems && items.length === 0) {
    items = initialItems
    setItems(initialItems);
    initialItems = null;
  }

  interface RowProps {
    editingIndex: number | null;
    index: number;
    item: any;
  }
  const RowItem : React.FC<RowProps> = ({editingIndex, index, item}) => {
    if (columns.some((col) => (col.visible !== undefined && col.visible === false) 
      || (typeof col.visible === "function" && !col.visible(item)))) {
      return <></>;
    }
    if (editingIndex === null || editingIndex !== index) {
      return columns.map((col) => (
            <td key={col.key}>
              {col.convert ? col.convert(item[col.key]) : (
              col.linkTo ? (
                <a href={col.linkTo(item.id)}>{item[col.key]}</a>
              ) : item[col.key]
              )}
            </td>
          ));
    }
    return columns.map((col) => 
      col.editable === false || editingIndex !== index ? (
      <td key={col.key}>
        {col.convert ? col.convert(editingItem[col.key]) : editingItem[col.key]}
      </td>
      ) : (
      <td key={col.key}>
        {typeof editingItem[col.key] === "number" ? (
          <Form.Control
            type="number"
            value={editingItem[col.key]}
            onChange={(e) => setEditingItem({ ...editingItem, [col.key]: parseFloat(e.target.value) })}/>
        ) : typeof editingItem[col.key] === "boolean" ? (
          <Form.Control
            as="select"
            value={editingItem[col.key] ? "true" : "false"}
            onChange={(e) => setEditingItem({ ...editingItem, [col.key]: e.target.value === "true" })}>
            <option value="true">True</option>
            <option value="false">False</option>
          </Form.Control>
        ) : col.options ? (
          <Form.Control
            as="select"
            value={editingItem[col.key]}
            onChange={(e) => setEditingItem({ ...editingItem, [col.key]: e.target.value })}>
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
            onChange={(e) => setEditingItem({ ...editingItem, [col.key]: e.target.value })}/>
        )}
      </td>));
  };

  return (
    <div>
      <Table striped bordered hover>
        <thead>
          <tr>
            {columns.map((col) => (
              <th key={col.key}>{col.label}</th>
            ))}
            {(deleteItem || editItem) && <th>Actions</th>}
          </tr>
        </thead>
        <tbody>
          {items.map((item, index) => (
            <tr key={index}>
              <RowItem editingIndex={editingIndex} index={index} item={item} />
              <td>
                {!!editItem && (editingIndex == null || editingIndex !== index) && (
                  <Button variant="outline-primary" className="me-2" onClick={() => { setEditingItem({...item}); setEditingIndex(index); }}>
                    <PencilSquare size={18} /> Edit
                  </Button>
                )}
                {!!deleteItem && (editingIndex === null || editingIndex !== index) && (
                  <Button variant="outline-danger" onClick={() => deleteItem(item)}>
                    <Trash size={18} /> Delete
                  </Button>
                )}
                {!!editItem && editingIndex === index && (
                  <Button variant="outline-primary" className="me-2" onClick={() => {
                      editItem(item);
                      items[index] = editingItem;
                      setItems([...items]);
                      setEditingIndex(null);
                      setEditingItem(null);
                    }}>
                    <Floppy size={18} /> Save
                  </Button>
                )}
                {!!editItem && editingIndex === index && (
                  <Button variant="outline-secondary" onClick={() => { setEditingIndex(null); setEditingItem(null)}}>
                   <X size={18} />  Cancel
                  </Button>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
      <div className="pagination-container">
        {prevItems && <Button
          variant="secondary"
          onClick={() => setprev(prev)}
          disabled={!prev}
          className="pagination-button left">
          <ChevronLeft size={20} />
        </Button>}
        {nextItems && <Button
          variant="primary"
          onClick={() => setnext(next)}
          disabled={!next}
          className="pagination-button right">
          <ChevronRight size={20} />
        </Button>}
      </div>
    </div>
  );
};


export default PaginatedList;
