import React from "react";
import api from "../../api";
import PaginatedList from "../../components/paginated-list/paginated-list-component";

const MyTeamsPage = () => {

  const next = async (offset?: number, limit?: number) => {
    const data = await api.std.team.mine();
    return data;
  };

  return (
   <div>
      <h2>My Teams </h2>
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

export default MyTeamsPage;
