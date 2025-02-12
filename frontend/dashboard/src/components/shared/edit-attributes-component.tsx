import React, { useState } from 'react';
import { Button, Form, Row, Col } from 'react-bootstrap';

interface EditAttributesProps {
  attributes: Map<string, number | string | boolean | Map<string, number | string | boolean>>;
  onSave: (attributes: Map<string, number | string | boolean | Map<string, number | string | boolean>>) => void;
}

const EditAttributes: React.FC<EditAttributesProps> = ({ attributes, onSave }) => {
  const [editedAttributes, setEditedAttributes] = useState(attributes);

  const handleAttributeChange = (key: string, value: string) => {
    setEditedAttributes(new Map(editedAttributes).set(key, value));
  };

  const handleSave = () => {
    onSave(editedAttributes);
  };

  return (
    <div>
      <h3>Edit Attributes</h3>
      {Array.from(editedAttributes.entries()).map(([key, value]) => (
        <Form.Group as={Row} key={key} className="mb-3">
          <Form.Label column sm="2">{key}</Form.Label>
          <Col sm="10">
            <Form.Control
              type="text"
              value={String(value)}
              onChange={(e) => handleAttributeChange(key, e.target.value)}
            />
          </Col>
        </Form.Group>
      ))}
      <Button variant="primary" onClick={handleSave}>Save</Button>
    </div>
  );
};

export default EditAttributes;