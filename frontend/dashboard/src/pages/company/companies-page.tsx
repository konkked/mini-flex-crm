import React from "react";
import api from "../../api";
import PaginatedList from "../../components/paginated-list/paginated-list-component";
import { Button } from "react-bootstrap";
import { Plus } from "react-bootstrap-icons";

const CompaniesPage = () => {
  
  const fetch = async (offset?: number, limit?: number) => {
    const data = await api.std.company.list(offset, limit);
    return data;
  };

  return (
    <div>
      <h2>Companies {" "} <Button className='add-btn' 
                                  variant="outline-secondary" onClick={()=>window.location.href='/company/new'}> 
                                  <Plus />
                          </Button> </h2>
      <PaginatedList
        fetch={fetch}
        columns={[
          { key: "id", label: "ID", linkTo: (id: number) => `/company/${id}` },
          { key: "name", label: "Name", editable: true },
        ]}
      />
    </div>
  );
};

export default CompaniesPage;