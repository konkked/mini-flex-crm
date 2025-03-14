import React from "react";
import api from "../../api";
import PaginatedList from "../../components/paginated-list/paginated-list-component";

const TenantsPage : React.FC = () => {
  const fetch = async (offset?: number, limit?: number) => {
    const data = await api.admin.tenant.list(offset, limit);
    return data;
  };

  return (
    <div>
      <h2>Tenants</h2>
      <PaginatedList
        fetch={fetch}
        columns={[
          { key: "id", label: "ID", linkTo: (id: number) => `/tenant/${id}` },
          { key: "name", label: "Name", editable: true },
        ]}
      />
    </div>
  );
};

export default TenantsPage;