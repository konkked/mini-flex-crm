import React from "react";
import api from "../../api";
import PaginatedList from "../../components/paginated-list/paginated-list-component";
import { Button } from "react-bootstrap";
import './tenants-page.css'
import { Plus } from "react-bootstrap-icons";

const TenantsPage : React.FC = () => {
  const fetch = async (offset?: number, limit?: number) => {
    const data = await api.admin.tenant.list(offset, limit);
    return data;
  };

  return (
    <div>
      <h2> 
        Tenants {" "} <Button className='add-btn' 
                                  variant="outline-secondary" onClick={()=>window.location.href='/tenant/new'}> 
                                  <Plus />
                          </Button> 
      </h2>
      <PaginatedList
        fetch={fetch}
        columns={[
          { key: "id", label: "ID", linkTo: (id: number) => `/tenant/${id}` },
          { key: "name", label: "Name", editable: true },
          { key: "shortId", label: "ShortId", editable: true},
          { key: "theme", label: "Portal Theme", editable: true}
        ]}
      />
    </div>
  );
};

export default TenantsPage;