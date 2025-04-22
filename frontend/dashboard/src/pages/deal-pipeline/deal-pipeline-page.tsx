import React, { useEffect, useState } from 'react';
import PipelineComponent from '../../components/pipeline/pipeline-component';
import api from '../../api';
import { Deal, DealStatusType } from '../../models/deal';
import './deal-pipeline-page.css';

const DealPipelinePage: React.FC = () => {
  const [deals, setDeals] = useState<Deal[]>([]);
  const [loading, setLoading] = useState<boolean>(true);

  const dealStates = {
    [DealStatusType.Abandoned]: 'Abandoned',
    [DealStatusType.Qualified]: 'Qualified',
    [DealStatusType.Outreach]: 'Outreach',
    [DealStatusType.Nurture]: 'Nurture',
    [DealStatusType.Closing]: 'Closing',
    [DealStatusType.Closed]: 'Closed',
  };

  useEffect(() => {
    const fetchDeals = async () => {
      try {
        const data = await api.std.deal.pipeline();
        setDeals(data);
      } catch (error) {
        console.error('Failed to fetch deals:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchDeals();
  }, []);

  const getState = (deal: Deal) => deal.status;
const setState = (deal: Deal, newState: string): Deal => ({ ...deal, status: newState as DealStatusType });
  const onStateUpdate = async (deal: Deal) => {
    try {
      await api.std.deal.edit(deal.id, deal);
      setDeals((prev) => prev.map((d) => (d.id === deal.id ? deal : d)));
    } catch (error) {
      console.error('Failed to update deal state:', error);
    }
  };

  const transitionCheck = (deal: Deal, newState: string) => {
    // Example: Prevent moving to "Closed Won" if the deal is in "Closed Lost" state
    if (deal.status === DealStatusType.Closing && newState !== DealStatusType.Closed) {
      return false;
    }
    return true;
  };

  const toCard = (deal: Deal) => ({
    id: deal.id,
    title: deal.name,
  });

  if (loading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="deal-pipeline-container">
      <div className="deal-pipeline-header">Deal Pipeline</div>
      <PipelineComponent<Deal>
        states={dealStates}
        items={deals}
        getState={getState}
        setState={setState}
        onStateUpdate={onStateUpdate}
        transitionCheck={transitionCheck}
        toCard={toCard}
      />
    </div>
  );
};

export default DealPipelinePage;