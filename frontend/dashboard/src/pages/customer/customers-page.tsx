import React from "react";
import api from "../../api";
import PaginatedList from "../../components/paginated-list/paginated-list-component";

const CustomersPage = () => {
  const next = async (offset?: number, limit?: number) => {
    const data = await api.std.customer.list(offset, limit);
    return data;
  };

  return (
    <div>
      <h2>Customers</h2>
      <PaginatedList
        fetch={next}
        columns={[
          { key: "id", label: "ID", linkTo: (id)=>`/customer/${id}`, editable: false },
          { key: "name", label: "ID", editable: false },
        ]}
      />
    </div>
  );
};

export default CustomersPage;
