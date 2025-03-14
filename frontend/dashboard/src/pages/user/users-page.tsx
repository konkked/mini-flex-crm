import React from "react";
import { Button, Container, Row, Col } from "react-bootstrap";
import { Link } from "react-router-dom";
import api, { hasAdminAccessToItem } from "../../api";
import PaginatedList from "../../components/paginated-list/paginated-list-component";
import './users-page.css'; // Import the CSS file

const UsersPage = () => {
  const fetch = async (offset?: number, limit?: number) => {
    const data = await api.std.user.list(offset, limit);
    return data;
  };

  return (
    <Container fluid className="users-page-container">
      <Row className="justify-content-between align-items-center mb-4">
        <Col>
          <h2>Users</h2>
        </Col>
        <Col className="text-end">
          <Link to="/user/add">
            <Button variant="primary">Add New User</Button>
          </Link>
        </Col>
      </Row>
      <PaginatedList
        fetch={fetch}
        columns={[
          { key: "id", label: "ID", editable: false, linkTo: (id: number) => `/user/${id}` },
          { key: "username", label: "Username" },
          { key: "name", label: "Name" },
          { key: "email", label: "Email" },
          { key: "role", 
            label: "Role", 
            editable: hasAdminAccessToItem,
            options: [
                { value: "admin", label: "Admin" }, 
                { value: "user", label: "User" }
            ],
          },
          { key: "enabled", label: "Enabled", editable: hasAdminAccessToItem, visible: hasAdminAccessToItem },
        ]}
      />
    </Container>
  );
};

export default UsersPage;