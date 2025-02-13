import React from "react";
import api from "../../api";
import PaginatedList from "../../components/paginated-list/paginated-list-component";

const CompaniesPage = () => {
  const next = async (token?: string) => {
    const data = await api.std.company.list.next(token);
    return { ...data };
  };

  const prev = async (token?: string) => {
    const data = await api.std.company.list.prev(token);
    return { ...data };
  };

  return (
    <div>
      <h2>Companies</h2>
      <PaginatedList
        nextItems={next}
        prevItems={prev}
        columns={[
          { key: "id", label: "ID", linkTo: (id: number) => `/company/${id}` },
          { key: "name", label: "Name", editable: true },
        ]}
      />
    </div>
  );
};

export default CompaniesPage;