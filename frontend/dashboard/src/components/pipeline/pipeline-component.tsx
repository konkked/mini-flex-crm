import React, { useState } from 'react';
import { useDrag, useDrop } from 'react-dnd';
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import './pipeline-component.css';

interface PipelineProps<T> {
  states: Record<string, string>; // Map of state IDs to titles
  items: T[]; // List of generic items
  getState: (item: T) => string; // Function to get the state of an item
  setState: (item: T, newState: string) => T; // Function to set the state of an item
  onStateUpdate: (item: T) => Promise<void>; // Backend update function
  transitionCheck?: (item: T, newState: string) => boolean; // Optional transition check
  toCard: (item: T) => PipelineCardProps; // Function to extract card props from the item
}

interface PipelineCardProps {
  id: number;
  title: string;
}

const PipelineComponent = <T,>({
  states,
  items,
  getState,
  setState,
  onStateUpdate,
  transitionCheck,
  toCard,
}: PipelineProps<T>) => {
  const [draggingItem, setDraggingItem] = useState<number | null>(null);
  const [errorItems, setErrorItems] = useState<number[]>([]);

  const lanes = Object.keys(states).map((stateId) => ({
    stateId,
    title: states[stateId],
    items: items.filter((item) => getState(item) === stateId),
  }));

  const handleDrop = async (item: T, newState: string) => {
    if (transitionCheck && !transitionCheck(item, newState)) {
      const card = toCard(item);
      setErrorItems((prev) => [...prev, card.id]);
      toast.error(`Cannot move "${card.title}" to "${states[newState]}"`);
      return;
    }

    const updatedItem = setState(item, newState);
    setErrorItems((prev) => prev.filter((id) => id !== toCard(item).id));
    await onStateUpdate(updatedItem);
  };

  return (
    <div className="pipeline-container">
      {lanes.map((lane) => (
        <PipelineLane<T>
          key={lane.stateId}
          stateId={lane.stateId}
          title={lane.title}
          items={lane.items}
          onDrop={handleDrop}
          draggingItem={draggingItem}
          setDraggingItem={setDraggingItem}
          errorItems={errorItems}
          toCard={toCard}
        />
      ))}
      <ToastContainer />
    </div>
  );
};

interface PipelineLaneProps<T> {
  stateId: string;
  title: string;
  items: T[];
  onDrop: (item: T, newState: string) => void;
  draggingItem: number | null;
  setDraggingItem: (id: number | null) => void;
  errorItems: number[];
  toCard: (item: T) => PipelineCardProps;
}

const PipelineLane = <T,>({
  stateId,
  title,
  items,
  onDrop,
  draggingItem,
  setDraggingItem,
  errorItems,
  toCard,
}: PipelineLaneProps<T>) => {
  const [, drop] = useDrop({
    accept: 'CARD',
    drop: (item: T) => onDrop(item, stateId),
  });

  return (
    <div className="pipeline-lane" ref={(node) => { if(node) drop(node); }}>
      <div className="pipeline-lane-title">{title}</div>
      {items.map((item) => {
        const card = toCard(item);
        return (
          <PipelineCard
            key={card.id}
            card={card}
            isDragging={draggingItem === card.id}
            setDraggingItem={setDraggingItem}
            isError={errorItems.includes(card.id)}
          />
        );
      })}
    </div>
  );
};

interface PipelineCardComponentProps {
  card: PipelineCardProps;
  isDragging: boolean;
  setDraggingItem: (id: number | null) => void;
  isError: boolean;
}

const PipelineCard: React.FC<PipelineCardComponentProps> = ({ card, setDraggingItem, isError }) => {
  const [{ isDragging: dragging }, drag] = useDrag({
    type: 'CARD',
    item: card,
    collect: (monitor) => ({
      isDragging: monitor.isDragging(),
    }),
    end: () => setDraggingItem(null),
  });

  return (
    <div
      className={`pipeline-card ${dragging ? 'dragging' : ''} ${isError ? 'error' : ''}`}
      ref={(node) => { if (node) drag(node); }}
      onMouseDown={() => setDraggingItem(card.id)}
    >
      {card.title}
    </div>
  );
};

export default PipelineComponent;