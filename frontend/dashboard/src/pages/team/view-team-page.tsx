import React, { useState, useEffect } from "react";
import api, { isSuperAdmin } from "../../api";
import { useParams } from "react-router-dom";
import PaginatedList from "../../components/paginated-list/paginated-list-component";
import { Team } from "../../models/team";
import { Row, Col } from "react-bootstrap";
import ViewAttributesComponent from "../../components/attributes/view-attributes-component";
import "./view-team-page.css"; // Updated to use view-team-page styles

const ViewTeamPage: React.FC = () => {
  const { teamId } = useParams<{ teamId: string }>();
  const [team, setTeam] = useState<Team | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      const data = await api.std.team.get(parseInt(teamId || "0"));
      setTeam(data);
    };
    fetchData();
  }, [teamId]);

  if (!team) {
    return <div>Loading...</div>;
  }

  return (
    <div className="view-team-container">
      <Row className="align-items-center mb-3">
        <Col>
          <h1>Team: {team.name}</h1>
        </Col>
      </Row>
      <Row>
        <Col><b>Owner</b>: {team.owner.name}</Col>
      </Row>
      {isSuperAdmin() && (
        <Row>
          <Col><b>Tenant</b>: {team.tenant}</Col>
        </Row>
      )}
      {team.attributes && Object.keys(team.attributes).length > 0 && (
        <Row>
          <Col>
            <ViewAttributesComponent target={team} />
          </Col>
        </Row>
      )}
      <Row className="mb-3">
        <h2>Accounts</h2>
        <PaginatedList
          fetch={async (_0, _1) => team.accounts ?? []}
          columns={[
            { key: "id", label: "ID" },
            { key: "name", label: "Name" },
          ]}
        />
      </Row>
      <Row className="mb-3">
        <h2>Members</h2>
        <PaginatedList
          fetch={async (_0, _1) => team.members ?? []}
          columns={[
            { key: "user.id", label: "ID" },
            { key: "user.name", label: "Name" },
            { key: "role", label: "Role" },
          ]}
        />
      </Row>
    </div>
  );
};

export default ViewTeamPage;
