import React, { useState } from "react";
import { Form, InputGroup, Button } from "react-bootstrap";
import { Search } from "react-bootstrap-icons";
import { toast } from "react-toastify";
import { useQueryState } from "../../hooks/useQueryState.ts";

const SearchBar = () => {
  const [search, setSearch] = useQueryState("search");
  const [searchText, setSearchText] = useState(search || "");

  const handleSearch = () => {
    if (!searchText.trim()) {
        setSearch(searchText);
        toast.success("Search applied!");
    } else {
        setSearch("");
        toast.success("Search cleared!");
    }
  };

  return (
    <InputGroup className="mb-3" style={{ width: "100%", padding: "1rem" }}>
      <Form.Control
        type="text"
        placeholder="Search..."
        value={searchText}
        onChange={(e) => setSearchText(e.target.value)}
      />
      <Button variant="primary" onClick={handleSearch}>
        <Search size={20} />
      </Button>
    </InputGroup>
  );
};

export default SearchBar;
