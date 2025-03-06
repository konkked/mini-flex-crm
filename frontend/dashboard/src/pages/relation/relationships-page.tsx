import React from "react";
import api, { hasAdminAccessToItem, getCurrentRole} from "../../api";
import PaginatedList from "../../components/paginated-list/paginated-list-component";
import { Relationship } from "../../models/relationship";

const RelationshipsPage : React.FC = () => {
    const fetch = async (offset?: number, limit?: number) => {
      return await api.std.relationship.list(offset, limit);
    };
    const deleteItem = async (item: Relationship) => {
        if(hasAdminAccessToItem(item)){
            await api.admin.relationship.delete(item.id);
        }
    }

  return (
    <div>
      <h2>Relationships </h2>
      <PaginatedList
        fetch={fetch}
        deleteItem={getCurrentRole() === "admin" ? deleteItem : undefined}
        columns={[
          { key: "id", label: "ID" },
          { key: "name", label: "Name", editable: true },
          { key: "entityName", label: "Entity Name", editable: false },
          { key: "customerName", label: "Customer Name", editable: false },
        ]}
      />
    </div>
  );
};

export default RelationshipsPage;