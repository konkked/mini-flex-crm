import React from "react";
import api from "../../lib/api";
import PaginatedList from "../../components/shared/paginated-list-component";

const CompaniesPage = () => {
  const next = async (token?: string) => {
    const data = await api.std.user.list.next(token);
    return { ...data };
  };

  const prev = async (token?: string) => {
    const data = await api.std.user.list.prev(token);
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

export default CompaniesPage;