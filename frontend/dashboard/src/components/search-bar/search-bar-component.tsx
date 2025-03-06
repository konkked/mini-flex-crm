// src/components/search-bar/SearchBar.tsx
import React, { useEffect, useState } from 'react';
import { Form, ListGroup } from 'react-bootstrap';
import { Search } from 'react-bootstrap-icons';
import './search-bar-component.css';

interface SearchBarProps<T> {
  searchApi: (searchTerm: string) => Promise<T[]>;
  placeholder?: string;
  displayField?: keyof T;
  onSelect?: (item: T) => void;
}

const SearchBar = <T extends { id: number | string }>({
  searchApi,
  placeholder = 'Search...',
  displayField = 'name' as keyof T,
  onSelect,
}: SearchBarProps<T>) => {
  const [searchText, setSearchText] = useState('');
  const [results, setResults] = useState<T[]>([]);
  const [selected, setSelected] = useState<T | null>(null);
  const [loading, setLoading] = useState(false);
  const [debounceTimer, setDebounceTimer] = useState<NodeJS.Timeout | null>(null);

  const handleInputChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setSearchText(value);

    if (debounceTimer) clearTimeout(debounceTimer);

    if (value.trim()) {
      const timer = setTimeout(async () => {
        setLoading(true);
        try {
          const data = await searchApi(value);
          setResults(data);
        } catch (err) {
          console.error('Search failed:', err);
          setResults([]);
        } finally {
          setLoading(false);
        }
      }, 500); // 500ms debounce
      setDebounceTimer(timer);
    } else {
      setResults([]);
    }
  };

  const handleSelect = (item: T) => {
    setSelected(item);
    setResults([]); // Clear results after selection
    setSearchText(''); // Clear input
    if (onSelect) onSelect(item);
  };

  useEffect(() => {
    return () => {
      if (debounceTimer) clearTimeout(debounceTimer);
    };
  }, [debounceTimer]);

  return (
    <div className="search-bar-container">
      <Form.Group className="mb-0 position-relative">
        <Form.Control
          type="text"
          placeholder={placeholder}
          value={searchText}
          onChange={handleInputChange}
          className="search-input"
        />
        <Search className="search-icon" size={20} />
        {results.length > 0 && (
          <ListGroup className="search-results">
            {results.map((item) => (
              <ListGroup.Item
                key={item.id}
                action
                onClick={() => handleSelect(item)}
                className="search-item"
              >
                {String(item[displayField])}
              </ListGroup.Item>
            ))}
          </ListGroup>
        )}
      </Form.Group>
      {selected && (
        <div className="selected-item mt-2 p-2 border rounded">
          Selected: {String(selected[displayField])}
        </div>
      )}
      {loading && <div className="mt-2">Searching...</div>}
    </div>
  );
};

export default SearchBar;