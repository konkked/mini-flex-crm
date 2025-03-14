import { Attributable } from 'models/attributable';
import React, { useState, useEffect } from 'react';
import { Button, Form, Row, Col } from 'react-bootstrap';
import './manage-attributes-component.css'

interface ManageAttributesComponentProps {
  target: Attributable; // Optional initial attributes
  onAttributesChange: (attributable: Attributable) => void; // Callback for updates
}

const ManageAttributesComponent: React.FC<ManageAttributesComponentProps> = ({
  target = {},
  onAttributesChange,
}) => {
  const [attributable, setAttributable] = useState<{ [key: string]: any }>(target);

  useEffect(() => {
    setAttributable(target);
  }, [target]);

  const handleAttributeChange = (index: number, field: 'name' | 'value', value: any) => {
    const newAttributes = { ...attributable.attributes };
    const keys = Object.keys(newAttributes);
    if (field === 'name') {
      const oldKey = keys[index];
      const newKey = value;
      newAttributes[newKey] = newAttributes[oldKey];
      delete newAttributes[oldKey];
    } else {
      newAttributes[keys[index]] = value;
    }
    setAttributable({attributes: newAttributes});
    onAttributesChange(newAttributes); // Notify parent of changes
  };

  const addAttribute = () => {
    setAttributable((prev) => ({ ...prev, '': '' }));
    onAttributesChange({ ...attributable.attributes, '': '' }); // Notify parent
  };

  const removeAttribute = (index: number) => {
    const newAttributes = { ...attributable.attributes };
    const keys = Object.keys(newAttributes);
    delete newAttributes[keys[index]];
    setAttributable({attributes: newAttributes});
    onAttributesChange(newAttributes); // Notify parent
  };

  const renderValueInput = (key: string, value: any, index: number) => {
    if (typeof value === 'boolean') {
      return (
        <Form.Check
          type="checkbox"
          checked={value}
          onChange={(e) => handleAttributeChange(index, 'value', e.target.checked)}
        />
      );
    } else if (typeof value === 'number') {
      return (
        <Form.Control
          type="number"
          value={value}
          onChange={(e) => handleAttributeChange(index, 'value', parseFloat(e.target.value))}
        />
      );
    } else {
      return (
        <Form.Control
          type="text"
          value={value}
          onChange={(e) => handleAttributeChange(index, 'value', e.target.value)}
        />
      );
    }
  };

  return (
    <Form.Group controlId="formAttributes" className="mb-3">
      <Form.Label>Attributes</Form.Label>
      {Object.keys(attributable.attributes).map((key, index) => (
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
            {renderValueInput(key, attributable.attributes[key], index)}
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