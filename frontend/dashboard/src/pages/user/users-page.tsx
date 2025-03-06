import React from "react";
import api, { hasAdminAccessToItem } from "../../api";
import PaginatedList from "../../components/paginated-list/paginated-list-component";

const UsersPage = () => {
    const fetch = async (offset?: number, limit?: number) => {
      const data = await api.std.user.list(offset, limit);
      return data;
    };

  return (
    <div>
      <h2>Users</h2>
      <PaginatedList
        fetch={fetch}
        columns={[
          { key: "id", label: "ID", editable: false },
          { key: "username", label: "Username" },
          { key: "name", label: "Name" },
          { key: "email", label: "Email" },
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