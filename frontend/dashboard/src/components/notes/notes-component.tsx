import React, { useState, useEffect } from 'react';
import { Button, Modal, Form, ListGroup } from 'react-bootstrap';
import { getNotes, createNote, deleteNote, togglePinNote } from '../../api';
import './notes-component.css';
import { Note } from 'models/note';

interface NotesComponentProps {
  route: string;
}

const NotesComponent: React.FC<NotesComponentProps> = ({ route }) => {
  const [show, setShow] = useState(false);
  const [notes, setNotes] = useState<Note[]>([]);
  const [newNote, setNewNote] = useState({ title: '', content: '' });
  const [loading, setLoading] = useState(false);
  const [pinnedNote, setPinnedNote] = useState<Note | null>(null);

  useEffect(() => {
    if (show) {
      loadNotes();
    } else {
      loadPinnedNote();
    }
  }, [show]);

  const loadNotes = async () => {
    setLoading(true);
    const data = await getNotes(route);
    setNotes(data);
    const pinned = data.find(note => note.pinned);
    setPinnedNote(pinned || null);
    setLoading(false);
  };

  const loadPinnedNote = async () => {
    const data = await getNotes(route);
  };

  const handleCreateNote = async () => {
    await createNote({ ...newNote, route, pinned: false });
    setNewNote({ title: '', content: '' });
    loadNotes();
  };

  const handleDeleteNote = async (id: number) => {
    await deleteNote(id);
    loadNotes();
  };

  const handlePinNote = async (id: number) => {
    await togglePinNote(id);
    loadNotes();
    loadPinnedNote();
  };

  return (
    <>
      <div className="notes-tag" onClick={() => setShow(true)}>
        <i className="bi bi-sticky"></i> Notes
      </div>
      {pinnedNote && !show && (
        <div className="pinned-note">
          <div className="note-title">{pinnedNote.title}</div>
          <Button variant="link" onClick={() => handlePinNote(pinnedNote?.id || 0)}>
                    <i className={`bi ${pinnedNote.pinned ? 'bi-pin-fill' : 'bi-pin'}`} style={{ color: pinnedNote.pinned ? 'dark' : 'light' }}></i>
          </Button>
          <div className="note-content">{pinnedNote.content}</div>
        </div>
      )}
      <Modal show={show} onHide={() => setShow(false)} className="notes-modal">
        <Modal.Header closeButton>
          <Modal.Title>Notes</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form.Group controlId="newNoteTitle">
            <Form.Label>Title</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter title"
              value={newNote.title}
              onChange={(e) => setNewNote({ ...newNote, title: e.target.value })}
            />
          </Form.Group>
          <Form.Group controlId="newNoteContent">
            <Form.Label>Content</Form.Label>
            <Form.Control
              as="textarea"
              rows={3}
              placeholder="Enter content"
              value={newNote.content}
              onChange={(e) => setNewNote({ ...newNote, content: e.target.value })}
            />
          </Form.Group>
          <Button variant="primary" onClick={handleCreateNote}>
            Add Note
          </Button>
          {loading && <div className="loading-indicator">Loading...</div>}
          <ListGroup>
            {notes.map((note) => (
              <ListGroup.Item key={note.id} className="note-item">
                <div className="note-header"></div>
                  <Button variant="link" className="delete-button" onClick={() => handleDeleteNote(note?.id || 0)}>
                    <i className="bi bi-x-circle"></i>
                  </Button>
                  <div className="note-title">{note.title}</div>
                  <Button variant="link" onClick={() => handlePinNote(note?.id || 0)}>
                    <i className={`bi ${note.pinned ? 'bi-pin-fill' : 'bi-pin'}`} style={{ color: note.pinned ? 'dark' : 'light' }}></i>
                  </Button>
                <div className="note-content">{note.content}</div>
              </ListGroup.Item>
            ))}
          </ListGroup>
        </Modal.Body>
      </Modal>
    </>
  );
};

export default NotesComponent;