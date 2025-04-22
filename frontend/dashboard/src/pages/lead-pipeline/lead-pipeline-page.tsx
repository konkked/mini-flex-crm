import React, { useEffect, useState } from 'react';
import PipelineComponent from '../../components/pipeline/pipeline-component';
import api from '../../api';
import { Lead, LeadStatusType } from '../../models/lead';
import './lead-pipeline-page.css';

const LeadPipelinePage: React.FC = () => {
  const [leads, setLeads] = useState<Lead[]>([]);
  const [loading, setLoading] = useState<boolean>(true);

  const leadStates = {
    [LeadStatusType.Raw]: 'Raw',
    [LeadStatusType.Bronze]: 'Bronze',
    [LeadStatusType.Silver]: 'Silver',
    [LeadStatusType.Gold]: 'Gold',
    [LeadStatusType.Qualified]: 'Qualified'
  };

  useEffect(() => {
    const fetchLeads = async () => {
      try {
        const data = await api.std.lead.pipeline();
        setLeads(data);
      } catch (error) {
        console.error('Failed to fetch leads:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchLeads();
  }, []);

  const getState = (lead: Lead) => lead.status;
  const setState = (lead: Lead, newState: string) : Lead => ({ ...lead, status: newState as LeadStatusType });
  const onStateUpdate = async (lead: Lead) => {
    try {
      await api.std.lead.edit(lead.id, lead);
      setLeads((prev) => prev.map((l) => (l.id === lead.id ? lead : l)));
    } catch (error) {
      console.error('Failed to update lead state:', error);
    }
  };

  const transitionCheck = (lead: Lead, newState: string) => {
    // Example: Prevent moving from "Qualified" once deal is qualified.
    if (lead.status === LeadStatusType.Qualified) {
      return false;
    }
    return true;
  };

  const toCard = (lead: Lead) => ({
    id: lead.id,
    title: lead.name,
  });

  if (loading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="lead-pipeline-container">
      <div className="lead-pipeline-header">Lead Pipeline</div>
      <PipelineComponent<Lead>
        states={leadStates}
        items={leads}
        getState={getState}
        setState={setState}
        onStateUpdate={onStateUpdate}
        transitionCheck={transitionCheck}
        toCard={toCard}
      />
    </div>
  );
};

export default LeadPipelinePage;