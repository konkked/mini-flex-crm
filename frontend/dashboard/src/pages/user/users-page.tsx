import React from "react";
import { Button, Container, Row, Col } from "react-bootstrap";
import { Link } from "react-router-dom";
import api, { hasAdminAccessToItem } from "../../api";
import PaginatedList from "../../components/paginated-list/paginated-list-component";
import './users-page.css'; // Import the CSS file
import { Plus } from "react-bootstrap-icons";

const UsersPage = () => {
  const fetch = async (offset?: number, limit?: number) => {
    const data = await api.std.user.list(offset, limit);
    return data;
  };

  const getInitials = (name: string) => {
    const parts = name.split(' ');
    if (parts.length >= 2) {
      return `${parts[0][0]}${parts[parts.length - 1][0]}`.toUpperCase();
    }
    return name.substring(0, 2).toUpperCase();
  };

  const renderProfileImage = (item: any) => {
    if (item.profileImage) {
      return (
        <div className="user-list-profile-image">
          <img 
            src={`data:image/png;base64,${item.profileImage}`} 
            alt={`${item.name}'s profile`} 
          />
        </div>
      );
    } else {
      return (
        <div className="user-list-profile-image-placeholder">
          {getInitials(item.name)}
        </div>
      );
    }
  };

  return (
    <div>
      <h2>Users {" "} <Button className='add-btn' 
                                        variant="outline-secondary" onClick={()=>window.location.href='/user/new'}> 
                                        <Plus />
                                </Button> </h2>
      <PaginatedList
        fetch={fetch}
        columns={[
          { 
            key: "profileImage", 
            label: "", 
            render: renderProfileImage,
            width: "60px"
          },
          { key: "id", label: "ID", editable: false, linkTo: (id: number) => `/user/${id}` },
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