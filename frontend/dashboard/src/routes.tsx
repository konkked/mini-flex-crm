import { BrowserRouter as Router, Routes, Route, Navigate, Outlet } from 'react-router-dom';
import LoginPage from './pages/auth/login-page';
import ManageUserPage from './pages/user/manage-user-page';
import { useAuth } from './hooks/useAuth';
import UsersPage from './pages/user/users-page';
import ManageCompanyPage from './pages/company/manage-company-page';
import CompaniesPage from './pages/company/companies-page';
import HomePage from './pages/home/home-page';
import RelationshipsPage from './pages/relationships/relationships-page';
import TenantsPage from './pages/tenant/tenants-page';
import ManageTenantPage from './pages/tenant/manage-tenant-page';
import ViewTenantPage from './pages/tenant/view-tenant-page';
import React from 'react';
import { getCurrentRole, getCurrentTenantId } from './api'; // Corrected import path assuming './api'
import ViewUserPage from './pages/user/view-user-page'; // Corrected import path
import UserProfilePage from './pages/user/user-profile-page'; // Import the user profile page
import AccountsPage from './pages/account/accounts-page';
import ManageAccountPage from './pages/account/manage-account-page';
import ViewCompanyPage from './pages/company/view-company-page';
import ViewAccountPage from './pages/account/view-account-page';
import DealPipelinePage from './pages/deal-pipeline/deal-pipeline-page'; // Import Deal Pipeline Page
import LeadPipelinePage from './pages/lead-pipeline/lead-pipeline-page'; // Import Lead Pipeline Page
import ManageTeamPage from './pages/team/manage-team-page';
import TeamsPage from './pages/team/teams-page';
import ViewTeamPage from './pages/team/view-team-page';
import MyTeamsPage from './pages/team/my-teams-page';

// PrivateRoute component to handle authentication and role-based access
const PrivateRoute: React.FC<{ managerOrAdminOnly?: boolean; adminOnly?: boolean; superAdminOnly?: boolean; }> = ({ managerOrAdminOnly = false, adminOnly = false, superAdminOnly = false }) => {
  const { isAuthenticated } = useAuth();
  const role = getCurrentRole();
  const tenantId = getCurrentTenantId();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (adminOnly && role !== 'admin') {
    return <Navigate to="/home" replace />;
  }

  if(managerOrAdminOnly && role !== 'manager' && role !== 'admin'){
    return <Navigate to="/home" replace />
  }

  if(superAdminOnly && (role !== "admin" || tenantId !=0)){
    return <Navigate to="/home" replace />
  }

  return <Outlet />;
};

const AppRoutes = () => {
  const { isAuthenticated } = useAuth();

  return (
    <Router>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route element={<PrivateRoute />}>
          <Route path="/users" element={<UsersPage />} />
          <Route path="/user/:userId" element={<ViewUserPage />} />
          <Route path="/user/:userId/profile" element={<UserProfilePage />} />
          <Route path="/companies" element={<CompaniesPage />} />
          <Route path="/company/:companyId" element={<ViewCompanyPage/>} />
          <Route path="/accounts" element={<AccountsPage />} />
          <Route path="/account/:customerId" element={<ViewAccountPage />} />
          <Route path="/account/:customerId/manage" element={<ManageAccountPage />} />
          <Route path="/account/new" element={<ManageAccountPage />} />
          <Route path="/relationships" element={<RelationshipsPage />} />
          <Route path="/lead-pipeline" element={<LeadPipelinePage />} /> 
          <Route path="/deal-pipeline" element={<DealPipelinePage />} /> 
          {isAuthenticated ? (
            <>
              <Route path="/home" element={<HomePage />} />
              <Route path="*" element={<Navigate to="/home" replace />} />
            </>
          ) : (
            <Route path="*" element={<Navigate to="/login" replace />} />
          )}
        </Route>
        <Route element={<PrivateRoute managerOrAdminOnly />}>
          <Route path="/team/:teamId/manage" element={<ManageTeamPage />} />
        </Route>
        <Route path="/teams/mine" element={ <MyTeamsPage /> } />
        <Route path="/teams" element={<TeamsPage />} />
        <Route path="/team/:teamId" element={<ViewTeamPage />} />
        <Route element={<PrivateRoute adminOnly />}>
          <Route path="/user/:userId/manage" element={<ManageUserPage />} />
          <Route path="/user/new" element={<ManageUserPage />} />
          <Route path="/company/:companyId/manage" element={<ManageCompanyPage />} />
          <Route path="/company/new" element={<ManageCompanyPage />} />
        </Route>
        <Route element={<PrivateRoute superAdminOnly />}>
          <Route path="/tenant/:tenantId/user/new" element={<ManageUserPage />} />
          <Route path="/tenants" element={<TenantsPage />} />
          <Route path="/tenant/new" element={<ManageTenantPage />} />
          <Route path="/tenant/:tenantId/manage" element={<ManageTenantPage />} />
          <Route path="/tenant/new" element={<ManageTenantPage />} />
          <Route path="/tenant/:tenantId" element={<ViewTenantPage />} />
        </Route>
      </Routes>
    </Router>
  );
};

export default AppRoutes;