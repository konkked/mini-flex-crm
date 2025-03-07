// src/components/attributes/attributes-component.tsx
import React, { useState } from 'react';
import { Button, Form, Row, Col } from 'react-bootstrap';

interface ManageAttributesComponentProps {
  initialAttributes?: { [key: string]: string }; // Optional initial attributes
  onAttributesChange: (attributes: { [key: string]: string }) => void; // Callback for updates
}

const ManageAttributesComponent: React.FC<ManageAttributesComponentProps> = ({
  initialAttributes = {},
  onAttributesChange,
}) => {
  const [attributes, setAttributes] = useState<{ [key: string]: string }>(initialAttributes);

  const handleAttributeChange = (index: number, field: 'name' | 'value', value: string) => {
    const newAttributes = { ...attributes };
    const keys = Object.keys(newAttributes);
    if (field === 'name') {
      const oldKey = keys[index];
      const newKey = value;
      newAttributes[newKey] = newAttributes[oldKey];
      delete newAttributes[oldKey];
    } else {
      newAttributes[keys[index]] = value;
    }
    setAttributes(newAttributes);
    onAttributesChange(newAttributes); // Notify parent of changes
  };

  const addAttribute = () => {
    setAttributes((prev) => ({ ...prev, '': '' }));
    onAttributesChange({ ...attributes, '': '' }); // Notify parent
  };

  const removeAttribute = (index: number) => {
    const newAttributes = { ...attributes };
    const keys = Object.keys(newAttributes);
    delete newAttributes[keys[index]];
    setAttributes(newAttributes);
    onAttributesChange(newAttributes); // Notify parent
  };

  return (
    <Form.Group controlId="formAttributes" className="mb-3">
      <Form.Label>Attributes</Form.Label>
      {Object.keys(attributes).map((key, index) => (
        <Row key={index} className="mb-2 align-items-center">
          <Col>
            <Form.Control
              type="text"
              placeholder="Attribute Name"
              value={key}
              onChange={(e) => handleAttributeChange(index, 'name', e.target.value)}
            />
          </Col>
          <Col>
            <Form.Control
              type="text"
              placeholder="Attribute Value"
              value={attributes[key]}
              onChange={(e) => handleAttributeChange(index, 'value', e.target.value)}
            />
          </Col>
          <Col xs="auto">
            <Button variant="danger" onClick={() => removeAttribute(index)}>
              Remove
            </Button>
          </Col>
        </Row>
      ))}
      <Button variant="secondary" onClick={addAttribute}>
        Add Attribute
      </Button>
    </Form.Group>
  );
};

export default ManageAttributesComponent;