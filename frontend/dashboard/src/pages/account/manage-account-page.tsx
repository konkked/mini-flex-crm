import React, { useState, useEffect } from "react";
import api, { hasAdminAccessToItem, getCurrentRole, getCurrentTenantId } from "../../api";
import { useParams } from 'react-router-dom';
import { Form, Button, Container, Row, Col, Alert } from "react-bootstrap";
import PaginatedList from "../../components/paginated-list/paginated-list-component";
import ManageAttributes from "../../components/attributes/manage-attributes-component";
import { Relationship, PivotedRelationships } from "../../models/relationship";
import { Account, AccountFormData } from "../../models/account";
import { Company } from "../../models/company";
import SearchModal from "../../components/search-modal/search-modal";
import { Plus } from "react-bootstrap-icons"; // Bootstrap Icons
import SearchBar from "@components/search-bar/search-bar-component";
import { Tenant } from "models/tenant";
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import './manage-customer-page.css'; // Import the CSS file

const ManageCustomerPage: React.FC = () => {
  const [showCompanyModal, setShowCompanyAddModal] = useState(false);
  const [showUserModal, setShowUserAddModal] = useState(false);
  const { customerId, urlTenantId } = useParams<{ customerId: string, urlTenantId: string }>();
  const [accountFormData, setAccountFormData] = useState<AccountFormData | null>(null);
  const [account, setAccount] =  useState<Account>();
  const [relationships, setRelationships] = useState<PivotedRelationships | null>(null);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitSuccess, setSubmitSuccess] = useState<string | null>(null);
  const [tenant, setTenant] = useState<Tenant | null>(null);
  const [errors, setErrors] = useState<{ [key: string]: string }>({});
  const tenantId = Number(urlTenantId || getCurrentTenantId());

  useEffect(() => {
    const fetchData = async () => {
      const data = await api.std.customer.get_with_relationships(parseInt(customerId || "0"));
      if (data.relationships) {
        setRelationships(data.relationships);
      }
      setAccount(data);
      setAccountFormData(data);
    };
    fetchData();
  }, [customerId]);

  if (!relationships || !accountFormData) {
    return <div>Loading...</div>;
  }

  const handleSelectCompany = async (company: Company) => {
    await api.std.relationship.create({ customerId: accountFormData.id, id: company.id, entityName: "company" });
    setShowCompanyAddModal(false);
    const updatedRelationships = await api.std.customer.get_with_relationships(parseInt(customerId || "0"));
    setRelationships(updatedRelationships.relationships);
  };

  const handleSelectUser = async (user: { id: number }) => {
    await api.std.relationship.create({ customerId: accountFormData.id, id: user.id, entityName: "user" });
    setShowUserAddModal(false);
    const updatedRelationships = await api.std.customer.get_with_relationships(parseInt(customerId || "0"));
    setRelationships(updatedRelationships.relationships);
  };

  const searchCompanies = async (criteria: string) => {
    return await api.std.company.search({ name: criteria, tenant_id: accountFormData.tenantId });
  };

  const searchUsers = async (criteria: string) => {
    return await api.std.user.search({ username: criteria, tenant_id: accountFormData.tenantId });
  };

  const deleteRelationship = async (item: Relationship) => {
    await api.std.relationship.delete(item.id);
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setAccountFormData((prev) => prev ? { ...prev, [name]: value } : null);
    if (errors[name]) setErrors((prev) => ({ ...prev, [name]: '' }));
  };

  const validateForm = () => {
    const newErrors: { [key: string]: string } = {};
    if (!accountFormData?.name?.trim()) newErrors.name = 'Name is required';
    if (!tenantId && !tenant) newErrors.tenantId = 'Tenant selection is required';
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) return;

    try {
      accountFormData.tenantId =  tenant?.id || tenantId;
      if(customerId){
        await api.std.customer.edit(customerId, accountFormData);
        accountFormData.id = Number(customerId);
      } else {
        var id = await api.std.customer.create(accountFormData);
        accountFormData.id = id;
      }
      toast.success(
        <div>
          Customer updated successfully! <a href={`/account/${accountFormData.id}`}>View Customer</a>
        </div>,
        {
          onClose: () => {
            setTimeout(() => {
              setErrors({});
            }, 3000);
          }
        }
      );
      setSubmitError(null);
    } catch (err) {
      setSubmitError("Error updating customer");
      setSubmitSuccess(null);
    }
  };

  return (
    <Container fluid className="manage-customer-container">
      <ToastContainer />
      <Row className="justify-content-center align-items-center min-vh-100">
        <Col md={8} className="manage-customer-form">
          <h2 className="text-center mb-4">Manage Customer</h2>
          {submitError && <Alert variant="danger">{submitError}</Alert>}
          {submitSuccess && <Alert variant="success">{submitSuccess}</Alert>}
          <Form onSubmit={handleSubmit}>
            <Form.Group controlId="formName" className="mb-3">
              <Form.Label>Name <span className="text-danger">*</span></Form.Label>
              <Form.Control
                type="text"
                name="name"
                value={accountFormData.name}
                onChange={handleChange}
                isInvalid={!!errors.name}
                required
              />
              <Form.Control.Feedback type="invalid">{errors.name}</Form.Control.Feedback>
            </Form.Group>
            {!tenantId && (
              <Form.Group controlId="formTenantId" className="mb-3">
                <Form.Label>Tenant <span className="text-danger">*</span></Form.Label>
                <SearchBar<Tenant>
                  searchApi={api.admin.tenant.searchByName}
                  placeholder="Search..."
                  displayField="name"
                  onSelect={(tenant) => setTenant(tenant)}
                />
                {errors.tenantId && <div className="invalid-feedback d-block">{errors.tenantId}</div>}
              </Form.Group>
            )}
            <ManageAttributes
              target={accountFormData}
              onAttributesChange={(c) => setAccountFormData({ ...accountFormData, attributes: { ...c.attributes } })}
            />
            <Button variant="primary" type="submit" className="w-100">
              Save Changes
            </Button>
          </Form>
          <h2 className="mt-4">Relationships</h2>
          {hasAdminAccessToItem(account!) && (
            <Button
              variant="primary"
              onClick={() => setShowCompanyAddModal(true)}
              style={{ marginLeft: "10px" }}
            >
              <Plus />
            </Button>
          )}
          <div>
            {relationships.company && (
              <div>
                <h3>Companies</h3>
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
            {relationships.user && (
              <div>
                <h3>Users</h3>
                <PaginatedList
                  fetch={async (_0, _1) => relationships.user ?? []}
                  deleteItem={getCurrentRole() === "admin" ? deleteRelationship : undefined}
                  columns={[
                    { key: "id", label: "ID" },
                    { key: "username", label: "Username" },
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
                <SearchModal
                  show={showUserModal}
                  onHide={() => setShowUserAddModal(false)}
                  onSelect={handleSelectUser}
                  searchApi={searchUsers}
                  placeholder="Enter user name"
                />
              </div>
            )}
          </div>
        </Col>
      </Row>
    </Container>
  );
};

export default ManageCustomerPage;
