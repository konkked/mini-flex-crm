import React, { useState, useEffect } from "react";
import api, { hasAdminAccessToItem, getCurrentRole, getCurrentTenantId, isSuperAdmin, hasAccessToEditTeam } from "../../api";
import { useParams } from 'react-router-dom';
import { Form, Button, Container, Row, Col, Alert } from "react-bootstrap";
import PaginatedList from "../../components/paginated-list/paginated-list-component";
import ManageAttributes from "../../components/attributes/manage-attributes-component";
import { Team, TeamMember, TeamMemberUser } from "../../models/team";
import SearchModal from "../../components/search-modal/search-modal";
import { Plus } from "react-bootstrap-icons"; // Bootstrap Icons
import SearchBar from "@components/search-bar/search-bar-component";
import { Tenant } from "models/tenant";
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import './manage-customer-page.css'; // Import the CSS file
import { Account } from "models/account";

const ManageCustomerPage: React.FC = () => {
  const [showAccountAddModal, setShowAccountAddModal] = useState(false);
  const [showUserModal, setShowUserAddModal] = useState(false);
  const { teamId, urlTenantId } = useParams<{ teamId: string, urlTenantId: string }>();
  const [formChanged, setFormChanged] = useState(false);
  const [team, setTeam] =  useState<Team>();
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitSuccess, setSubmitSuccess] = useState<string | null>(null);
  const [tenant, setTenant] = useState<Tenant | null>(null);
  const [errors, setErrors] = useState<{ [key: string]: string }>({});
  const tenantId = Number(urlTenantId || getCurrentTenantId());

  useEffect(() => {
    const fetchData = async () => {
      const data = await api.std.team.get(parseInt(teamId || "0"));
      setTeam(data);
      if(isSuperAdmin()){
        setTenant(await api.admin.tenant.get(data.tenantId));
      }
    };
    fetchData();
  }, [teamId]);

  if (!team) {
    return <div>Loading...</div>;
  }

  const searchAccounts = async (criteria: string) => {
    return await api.std.account.search({ name: criteria });
  };

  const handleSelectAccount = async (account: Account) => {
    setFormChanged(true);
    setTeam({...team, accounts: [...team.accounts!, { id: account.id, name: account.name }]});
  };

  const removeAccount = async (item: { id:number }) => {
    setFormChanged(true);
    setTeam({...team, accounts: team.accounts?.filter((a) => a.id !== item.id)});
  };

  const removeMember = async (item: { id:number }) => {
    setFormChanged(true);
    setTeam({...team, members: team.members?.filter((a) => a.user.id !== item.id)});
  };

  const handleSelectUser = async (teamMember: {id: number, name:string, role:string, user:TeamMemberUser}) => {
    if(teamMember.role === "owner") {
      setFormChanged(true);
      setTeam({...team,  owner:teamMember.user});      
    } else {
      setFormChanged(true);
      setTeam({...team, members:[...team.members!, {role: teamMember.role, user: { id: teamMember.id,  name: teamMember.name, email: teamMember.user.email ?? "" }}]});
    }
  };

  const searchUsers = async (criteria: string) : Promise<{id: number, name:string, role:string, user:TeamMemberUser}[]> => {
    return (await api.std.team.searchPotentialMembers({ name: criteria }))
      .map((member: TeamMember) => { return { id: member.user.id, name: member.user.name, role: member.role, user: member.user };});
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormChanged(true);
    const { name, value } = e.target;
    setTeam({ ...team, [name]: value });
    if (errors[name]) setErrors((prev) => ({ ...prev, [name]: '' }));
  };

  const validateForm = () => {
    const newErrors: { [key: string]: string } = {};
    if (!team?.name?.trim()) newErrors.name = 'Name is required';
    if (!tenantId && !tenant) newErrors.tenantId = 'Tenant selection is required';
    if (!team?.owner) newErrors.owner = 'Owner selection is required';
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!formChanged || !validateForm()) return;

    try {
     await api.std.team.edit(team);
      toast.success(
        <div>
          Team updated successfully! <a href={`/team/${team.id}`}>View Team</a>
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
      setFormChanged(false);
    } catch (err) {
      setSubmitError("Error updating customer");
      setSubmitSuccess(null);
    }
  };

  return (
    <Container fluid className="manage-team-container">
      <ToastContainer />
      <Row className="justify-content-center align-items-center min-vh-100">
        <Col md={8} className="manage-team-form">
          <h2 className="text-center mb-4">Manage Team</h2>
          {submitError && <Alert variant="danger">{submitError}</Alert>}
          {submitSuccess && <Alert variant="success">{submitSuccess}</Alert>}
          <Form onSubmit={handleSubmit}>
            <Form.Group controlId="formName" className="mb-3">
              <Form.Label>Name <span className="text-danger">*</span></Form.Label>
              <Form.Control
                type="text"
                name="name"
                value={team.name}
                onChange={handleChange}
                isInvalid={!!errors.name}
                required
              />
              <Form.Control.Feedback type="invalid">{errors.name}</Form.Control.Feedback>
            </Form.Group>
            {isSuperAdmin() && (
              <Form.Group controlId="formTenantId" className="mb-3">
                <Form.Label>Tenant <span className="text-danger">*</span></Form.Label>
                <SearchBar<Tenant>
                  searchApi={api.admin.tenant.searchByName}
                  placeholder="Search..."
                  displayField="name"
                  onSelect={(tenant) => { 
                      setTenant(tenant); 
                      team.tenantId = tenant.id; 
                      team.name = tenant.name; 
                      setTeam({...team, tenantId:tenant.id, tenant:tenant.name});
                      setFormChanged(true); 
                    } 
                  }
                  initialSelected={tenant}
                />
                {errors.tenantId && <div className="invalid-feedback d-block">{errors.tenantId}</div>}
              </Form.Group>
            )}
            <ManageAttributes
              target={team}
              onAttributesChange={(c) => { 
                setFormChanged(true);
                setTeam({ ...team, attributes: { ...c.attributes } });
              }}
            />
            <Button variant="primary" type="submit" className="w-100">
              Save Changes
            </Button>
          <h2 className="mt-4">Accounts
          {hasAccessToEditTeam(team) && (
            <Button
              variant="primary"
              onClick={() => setShowAccountAddModal(true)}
              style={{ marginLeft: "10px" }}>
              <Plus />
            </Button>
          )}</h2>
          </Form>
          <div>

          <SearchModal
              show={showUserModal}
              onHide={() => setShowUserAddModal(false)}
              onSelect={handleSelectUser}
              searchApi={searchUsers}
              placeholder="Enter member name"
            />
            {team.members && (
              <div>
                <h3>Members</h3>
                <PaginatedList
                  fetch={async (_0, _1) => team.members?.map(a=>{a.role, a.user.name, a.user.name, a.user.id}) ?? []}
                  deleteItem={hasAccessToEditTeam(team) ? removeMember : undefined}
                  columns={[
                    { key: "id", label: "ID" },
                    { key: "name", label: "Name" },
                    { key: "role", label: "Role" },
                  ]}
                />
              </div>
            )}

          <SearchModal
              show={showAccountAddModal}
              onHide={() => setShowAccountAddModal(false)}
              onSelect={handleSelectAccount}
              searchApi={searchAccounts}
              placeholder="Enter Account name"
            />
            {team.accounts && (
              <div>
                <h3>Accounts</h3>
                <PaginatedList
                  fetch={async (_0, _1) => team.accounts ?? []}
                  deleteItem={hasAccessToEditTeam(team) ? removeAccount : undefined}
                  columns={[
                    { key: "id", label: "ID" },
                    { key: "name", label: "Name", editable: true },
                  ]}
                />
                <SearchModal
                  show={showUserModal}
                  onHide={() => setShowUserAddModal(false)}
                  onSelect={handleSelectUser}
                  searchApi={searchUsers}
                  placeholder="Enter user name"
                  display={i=>`Name: ${i.name} | Role: ${i.role}`}
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
