import { Attributable } from 'models/attributable';
import React, { useState } from 'react';
import { Row, Col } from 'react-bootstrap';
import './view-attributes-component.css';

interface ViewAttributesComponentProps {
  target: Attributable | null; // Attributes to display
}

const ViewAttributesComponent: React.FC<ViewAttributesComponentProps> = ({target}) => {
    target = target && target.attributes || { attributes: {}};
  return (
    <div>
      <h5>Attributes</h5>
      {Object.keys({...target.attributes}).map((key, index) => (
        <Row key={index} className="mb-2 align-items-center">
          <Col>
            <strong>{key} :</strong>
          </Col>
          <Col>
            {target.attributes ? target.attributes[key] : 'N/A'}
          </Col>
        </Row>
      ))}
    </div>
  );
};

export default ViewAttributesComponent;