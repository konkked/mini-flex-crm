import React, { useState, useEffect } from "react";
import api, {hasAdminAccessToItem, getCurrentRole} from "../../api";
import { useParams } from 'react-router-dom';
import PaginatedList from "../../components/paginated-list/paginated-list-component";
import EditAttributes from "../../components/shared/edit-attributes-component";
import { Relation, Relationships } from "../../models/relation";
import { Customer } from "../../models/customer";
import { Company } from "../../models/company";
import SearchModal from "../../components/shared/search-modal";
import { Button } from "react-bootstrap";
import { Plus } from "react-bootstrap-icons"; // Bootstrap Icons

const CustomerPage : React.FC =  () => {  
  const [showCompanyModal, setShowCompanyAddModal] = useState(false);
  const [showUserModal, setShowUserAddModal] = useState(false);
  const { customerId } = useParams<{ customerId: string }>();
  const [customer, setCustomer] = useState<Customer | null>();
  const [relationships, setRelationships] = useState<Relationships | null>(null);

  useEffect(()=>{
    const fetchData = async () => {
      const data = await api.std.customer.get_with_relationships(parseInt(customerId || "0"));
      if(data.relationships){
        setRelationships(data.relationships)
      }
      setCustomer(data);
    }
    fetchData();
  },[customerId])

  if(!relationships || !customer){
    return <div>Loading... </div>
  }

  const handleSelectCompany = async (company: Company) => {
    await api.std.relation.create({ customerId: customer.id, id: company.id, entityName: "company" });  
    setShowCompanyAddModal(false);
    const updatedRelationships = await api.std.customer.get_with_relationships({ customerId: customer.id });
    setRelationships(updatedRelationships.data.relationships);
  };

  const handleSelectUser = async (user: {id:number}) => {
    await api.std.relation.create({ customerId: customer.id, id: user.id, entityName: "user" });
    setShowCompanyAddModal(false);
    const updatedRelationships = await api.std.customer.get_with_relationships({ customerId: customer.id });
    setRelationships(updatedRelationships.data.relationships);
  };

  const searchCompanies = async (criteria: string) => {
    const data = await api.std.company.search({name: criteria, tenant_id: customer.tenantId});
    return data.items;
  }

  const searchUsers = async (criteria: string) => {
    const data = await api.std.company.search({username: criteria, tenant_id: customer.tenantId});
    return data.items;
  }

  const deleteRelation = async (item: Relation) => {
    await api.admin.relation.delete(item.id);
  }

  const handleSave = async (attributes: Map<string, number | string | boolean | Map<string, number | string | boolean>>) => {
    if (customer) {
      const updatedUser = { ...customer, attributes };
      await api.std.customer.update(customer.id, updatedUser);
      setCustomer(updatedUser);
    }
  };

  return (
  <div>
    <div style={{ display: "flex", alignItems: "center" }}>
      <h1>Company</h1>
      <EditAttributes 
        attributes={customer.attributes}
        onSave={handleSave}
      />
      <h2>Relationships</h2>
      {hasAdminAccessToItem(customer) && (
      <Button 
        variant="primary" 
        onClick={() => setShowCompanyAddModal(true)} 
        style={{ marginLeft: "10px" }}>
        <Plus />
      </Button>
      )}
    </div>
    <div>
    {relationships.company && (<div>
      <h3>Companies </h3>
      <PaginatedList
      initialItems={relationships.company}
      deleteItem={getCurrentRole() === "admin" ? deleteRelation : undefined}
      columns={[
        { key: "id", label: "ID" },
        { key: "name", label: "Name", editable: true },
      ]} />
      </div>)}
    
    <SearchModal
      show={showCompanyModal}
      onHide={() => setShowCompanyAddModal(false)}
      onSelect={handleSelectCompany}
      searchApi={searchCompanies}
      placeholder="Enter company name"
    />
    
    {relationships.user && (<div>
      <h3>Users </h3>
      <PaginatedList
        initialItems={relationships.user}
        deleteItem={getCurrentRole() === "admin" ? deleteRelation : undefined}
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
        ]} />

          <SearchModal
            show={showUserModal}
            onHide={() => setShowUserAddModal(false)}
            onSelect={handleSelectUser}
            searchApi={searchUsers}
            placeholder="Enter company name"
          />
        </div>)}
      </div>
    </div>);

};

const CustomersPage = () => {
  const next = async (token?: string) => {
    const data = await api.std.customer.list.next(token);
    return { items: data.customers, nextToken: data.nextToken };
  };

  const prev = async (token?: string) => {
    const data = await api.std.customer.list.prev(token);
    return { items: data.customers, nextToken: data.nextToken };
  }

  return (
    <div>
      <h2>Customers</h2>
      <PaginatedList
        nextItems={next}
        prevItems={prev}
        columns={[
          { key: "id", label: "ID", editable: false },
          { key: "name", label: "ID", editable: false },
        ]}
      />
    </div>
  );
};


export default CustomersPage;
