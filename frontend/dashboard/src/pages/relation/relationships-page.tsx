import React from "react";
import api, { hasAdminAccessToItem, getCurrentRole} from "../../api";
import PaginatedList from "../../components/paginated-list/paginated-list-component";
import { Relation } from "../../models/relation";

const RelationshipsPage : React.FC = () => {
    const next = async (token?: string) => {
      const data = await api.std.relation.list.next(token);
      return { ...data };
    };
    const prev = async (token?: string) => {
      const data = await api.std.relation.list.prev(token);
      return { ...data };
    }
    const deleteItem = async (item: Relation) => {
        if(hasAdminAccessToItem(item)){
            await api.admin.relation.delete(item.id);
        }
    }

  return (
    <div>
      <h2>Relationships </h2>
      <PaginatedList
        nextItems={next}
        prevItems={prev}
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