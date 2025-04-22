import React from "react";
import api, { getCurrentUser } from "../../api";
import PaginatedList from "../../components/paginated-list/paginated-list-component";
import { Form, Button, Modal, Toast } from "react-bootstrap";
import { Plus } from "react-bootstrap-icons";
import './accounts-page'
import { toast, ToastContainer } from "react-toastify";
import SearchBar from "../../components/search-bar/search-bar-component";
import { User } from "models/user";

const TeamsPage = () => {
  const [showAddModal, setShowAddModal] = React.useState(false);
  const [teamName, setTeamName] = React.useState("");
  const [teamOwner, setTeamOwner] = React.useState<User | null>(null);

  const next = async (offset?: number, limit?: number) => {
    const data = await api.std.team.list(offset, limit);
    return data;
  };

  const isAdminOrManager = () => {
    const user = getCurrentUser();
    return user?.role == "admin" || user?.role =="manager" || user?.role  == "group_manager";
  }

  return (
    <div>
      <ToastContainer />
        <Modal show={showAddModal} onHide={()=> setShowAddModal(false)}>
        <Modal.Header closeButton>
          <Modal.Title>Create Team</Modal.Title>
        </Modal.Header>
        <Modal.Body>
        <Form>
          <Form.Group className="mb-3" controlId="formBasicEmail">
            <Form.Label>Team Name</Form.Label>
            <Form.Control type="string" placeholder="Enter name" value={teamName} onChange={(e)=>{ 
                e.preventDefault(); 
                const { value } = e.target;
                setTeamName(value);
                }
              } />
            </Form.Group>
          </Form>
          <Form.Group className="mb-3" controlId="formBasicEmail">
            <Form.Label>Team Owner</Form.Label>
            <SearchBar<User>
              placeholder="Search for owner"
              searchApi={async (criteria: string) => {
                return await api.std.team.searchPotentialOwners({ name: criteria });
              }}
              onSelect={async (item) => {
                setTeamOwner(item);
              }} 
              />
            </Form.Group>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={()=> setShowAddModal(false)}>
            Close
          </Button>
          <Button variant="primary" disabled={teamOwner!==null && teamName !== ""} onClick={async ()=>{
            const id = await api.std.team.create({ name: teamName, ownerId: teamOwner?.id });
            toast.success(
              <div>
                Team updated successfully! <a href={`/team/${id}`}>View Team</a>
              </div>,
              {
                onClose: () => {
                  setTimeout(() => {
                    setShowAddModal(false);
                    setTeamName("");
                    setTeamOwner(null);
                  }, 500);
                }
              }
            );
          }}>
            Save Changes
          </Button>
        </Modal.Footer>
      </Modal>
      <h2>Teams {" "} {isAdminOrManager() && <Button className='add-btn' 
                                  variant="outline-secondary" onClick={()=>setShowAddModal(true)}> 
                                  <Plus />
                          </Button>} </h2>
      <PaginatedList
        fetch={next}
        columns={[
          { key: "id", label: "ID", linkTo: (id)=>`/team/${id}`, editable: false },
          { key: "name", label: "ID", editable: false },
        ]}
      />
    </div>
  );
};

export default TeamsPage;
