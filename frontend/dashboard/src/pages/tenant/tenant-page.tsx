import React from "react";
import api from "../../api";
import PaginatedList from "../../components/paginated-list/paginated-list-component";

const TenantsPage = () => {
  const next = async (token?: string) => {
    const data = await api.admin.tenant.list.next(token);
    return { ...data };
  };

  const prev = async (token?: string) => {
    const data = await api.admin.tenant.list.prev(token);
    return { ...data };
  };

  return (
    <div>
      <h2>Tenants</h2>
      <PaginatedList
        nextItems={next}
        prevItems={prev}
        columns={[
          { key: "id", label: "ID" },
          { key: "name", label: "Name", editable: true },
        ]}
      />
    </div>
  );
};

export default TenantsPage;