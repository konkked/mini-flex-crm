import React from 'react';
import { Row, Col } from 'react-bootstrap';

interface ViewAttributesComponentProps {
  attributes: { [key: string]: string }; // Attributes to display
}

const ViewAttributesComponent: React.FC<ViewAttributesComponentProps> = ({ attributes }) => {
  return (
    <div>
      <h5>Attributes</h5>
      {Object.keys(attributes).map((key, index) => (
        <Row key={index} className="mb-2 align-items-center">
          <Col>
            <strong>{key} :</strong>
          </Col>
          <Col>
            {attributes[key]}
          </Col>
        </Row>
      ))}
    </div>
  );
};

export default ViewAttributesComponent;