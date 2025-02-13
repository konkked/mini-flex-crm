import React from "react";
import api, { hasAdminAccessToItem } from "../../api";
import PaginatedList from "../../components/paginated-list/paginated-list-component";

const UsersPage = () => {
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
      <h2>Users</h2>
      <PaginatedList
        nextItems={next}
        prevItems={prev}
        columns={[
          { key: "id", label: "ID", editable: false },
          { key: "username", label: "Username" },
          { key: "role", 
            label: "Role", 
            editable: hasAdminAccessToItem,
            options: [
                { value: "admin", label: "Admin" }, 
                { value: "user", label: "User" }
            ],
          },
          { key: "enabled", label: "Enabled", editable: hasAdminAccessToItem, visible: hasAdminAccessToItem },
        ]}
      />
    </div>
  );
};

export default UsersPage;