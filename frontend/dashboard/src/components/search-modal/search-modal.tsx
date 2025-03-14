import React, { useState, useEffect } from 'react';
import { Modal, Button, Form, ListGroup } from 'react-bootstrap';
import './search-modal.css';

interface SearchModalProps<T> {
  show: boolean;
  onHide: () => void;
  onSelect: (item: T) => void;
  searchApi: (searchTerm: string) => Promise<T[]>;
  placeholder: string;
}

const SearchModal = <T extends { id: number; name: string }>({
  show,
  onHide,
  onSelect,
  searchApi,
  placeholder,
}: SearchModalProps<T>) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [results, setResults] = useState<T[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const delayDebounceFn = setTimeout(async () => {
      if (searchTerm) {
        setLoading(true);
        const data = await searchApi(searchTerm);
        setResults(data);
        setLoading(false);
      }
    }, 1000);

    return () => clearTimeout(delayDebounceFn);
  }, [searchTerm, searchApi]);

  return (
    <Modal show={show} onHide={onHide}>
      <Modal.Header closeButton>
        <Modal.Title>Search</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <Form.Group controlId="searchTerm">
          <Form.Label>Search</Form.Label>
          <Form.Control
            type="text"
            placeholder={placeholder}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </Form.Group>
        {loading && <div>Loading...</div>}
        <ListGroup>
          {results.map((item) => (
            <ListGroup.Item key={item.id} action onClick={() => onSelect(item)}>
              {item.name}
            </ListGroup.Item>
          ))}
        </ListGroup>
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={onHide}>
          Close
        </Button>
      </Modal.Footer>
    </Modal>
  );
};

export default SearchModal;