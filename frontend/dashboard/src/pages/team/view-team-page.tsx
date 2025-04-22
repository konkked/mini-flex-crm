import React, { useState, useEffect } from "react";
import api, { hasAdminAccessToItem, getCurrentRole } from "../../api";
import { useParams } from 'react-router-dom';
import PaginatedList from "../../components/paginated-list/paginated-list-component";
import { Relationship, PivotedRelationships } from "../../models/relationship";
import { Account } from "../../models/account";
import { Company } from "../../models/company";
import SearchModal from "../../components/search-modal/search-modal";
import { Button, Row, Col } from "react-bootstrap";
import { Plus } from "react-bootstrap-icons"; // Bootstrap Icons
import ViewAttributesComponent from "../../components/attributes/view-attributes-component";
import './view-customer-page.css';

const ViewCustomerPage: React.FC = () => {
  const [showCompanyModal, setShowCompanyAddModal] = useState(false);
  const [showUserModal, setShowUserAddModal] = useState(false);
  const { customerId } = useParams<{ customerId: string }>();
  const [customer, setCustomer] = useState<Account | null>(null);
  const [relationships, setRelationships] = useState<PivotedRelationships | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      const data = await api.std.customer.get_with_relationships(parseInt(customerId || "0"));
      if (data.relationships) {
        setRelationships(data.relationships);
      }
      setCustomer(data);
    };
    fetchData();
  }, [customerId]);

  if (!relationships || !customer) {
    return <div>Loading...</div>;
  }

  const handleSelectCompany = async (company: Company) => {
    await api.std.relationship.create({ customerId: customer.id, id: company.id, entityName: "company" });
    setShowCompanyAddModal(false);
    const updatedRelationships = await api.std.customer.get_with_relationships(parseInt(customerId || "0"));
    setRelationships(updatedRelationships.relationships);
  };

  const handleSelectUser = async (user: { id: number }) => {
    await api.std.relationship.create({ customerId: customer.id, id: user.id, entityName: "user" });
    setShowUserAddModal(false);
    const updatedRelationships = await api.std.customer.get_with_relationships(parseInt(customerId || "0"));
    setRelationships(updatedRelationships.relationships);
  };

  const searchCompanies = async (criteria: string) => {
    return await api.std.company.search({ name: criteria, tenant_id: customer.tenantId });
  };

  const searchUsers = async (criteria: string) => {
    return await api.std.user.search({ username: criteria, tenant_id: customer.tenantId });
  };

  const deleteRelationship = async (item: Relationship) => {
    await api.admin.relationship.delete(item.id);
  };

  return (
    <div className="view-customer-container">
      <Row className="align-items-center mb-3">
        <Col>
          <h1>Customer</h1>
        </Col>
        </Row>
        <Row>
          <Col><b>Name</b> : {customer.name}</Col>
        </Row>
        {customer.tenantName && <Row>
          <Col><b>Tenant</b> : {customer.tenantName}</Col>
        </Row>}
        {customer.attributes && Object.keys(customer.attributes).length > 0 && <Row>
        <Col>
          <ViewAttributesComponent target={customer} />
        </Col>
      </Row>}
      <Row className="mb-3">
        <Row>
          <h2>Relationships</h2>
        </Row>
        {hasAdminAccessToItem(customer) && (
            <Row className="align-items-center mb-2">
              <Col className="align-items-center mb-2">
            {!relationships.company && <Button variant="primary" onClick={() => setShowCompanyAddModal(true)} className="small-button" size="sm">
              <Plus /> Add Company
            </Button>}
            {!relationships?.user && <Button variant="primary" onClick={() => setShowUserAddModal(true)} className="small-button" size="sm">
              <Plus /> Add User
            </Button>}
            </Col>
            </Row>
        )}
      </Row>
      <Row>
        <Col>
          {relationships.company && (
            <div>
              <h3>Companies {" "} 
                <Button variant="primary" onClick={() => setShowCompanyAddModal(true)}>
                  <Plus />
                </Button>
              </h3>
              <PaginatedList
                fetch={async (_0, _1) => relationships.company ?? []}
                deleteItem={getCurrentRole() === "admin" ? deleteRelationship : undefined}
                columns={[
                  { key: "id", label: "ID" },
                  { key: "name", label: "Name", editable: true },
                ]}
              />
            </div>
          )}
          <SearchModal
            show={showCompanyModal}
            onHide={() => setShowCompanyAddModal(false)}
            onSelect={handleSelectCompany}
            searchApi={searchCompanies}
            placeholder="Enter company name"
          />
        </Col>
      </Row>
      <Row>
        <Col>
          {relationships.user && (
            <div>
              <h3>Users {" "} <Button variant="primary" onClick={() => setShowUserAddModal(true)} className="ms-2">
                  <Plus /> 
                </Button>
              </h3>
              <PaginatedList
                fetch={async (_0, _1) => relationships.user ?? []}
                deleteItem={getCurrentRole() === "admin" ? deleteRelationship : undefined}
                columns={[
                  { key: "id", label: "ID" },
                  { key: "username", label: "Username" },
                  {
                    key: "role",
                    label: "Role",
                    editable: hasAdminAccessToItem,
                    options: [
                      { value: "admin", label: "Admin" },
                      { value: "user", label: "User" },
                    ],
                  },
                  { key: "enabled", label: "Enabled", editable: hasAdminAccessToItem, visible: hasAdminAccessToItem },
                ]}
              />
              <SearchModal
                show={showUserModal}
                onHide={() => setShowUserAddModal(false)}
                onSelect={handleSelectUser}
                searchApi={searchUsers}
                placeholder="Enter user name"
              />
            </div>
          )}
        </Col>
      </Row>
    </div>
  );
};

export default ViewCustomerPage;
