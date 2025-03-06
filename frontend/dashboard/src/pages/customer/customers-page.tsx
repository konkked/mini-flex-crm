import React, { useState, useEffect } from "react";
import api, {hasAdminAccessToItem, getCurrentRole} from "../../api";
import { useParams } from 'react-router-dom';
import PaginatedList from "../../components/paginated-list/paginated-list-component";
import EditAttributes from "../../components/shared/edit-attributes-component";
import { Relationship, PivotedRelationships } from "../../models/relationship";
import { Customer } from "../../models/customer";
import { Company } from "../../models/company";
import SearchModal from "../../components/shared/search-modal";
import { Button } from "react-bootstrap";
import { Plus } from "react-bootstrap-icons"; // Bootstrap Icons

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
          { key: "id", label: "ID", editable: false },
          { key: "name", label: "ID", editable: false },
        ]}
      />
    </div>
  );
};

export default CustomersPage;
