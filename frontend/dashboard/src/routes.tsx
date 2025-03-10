import { BrowserRouter as Router, Routes, Route, Navigate, Outlet } from 'react-router-dom';
import LoginPage from './pages/auth/login-page';
import ManageUserPage from './pages/user/manage-user-page';
import { useAuth } from './hooks/useAuth';
import UsersPage from './pages/user/users-page';
import ManageCompanyPage from './pages/company/edit-company-page';
import CompaniesPage from './pages/company/companies-page';
import HomePage from './pages/home/home-page';
import RelationshipsPage from './pages/relation/relationships-page';
import TenantsPage from './pages/tenant/tenants-page';
import ManageTenantPage from './pages/tenant/manage-tenant-page';
import ViewTenantPage from './pages/tenant/view-tenant-page';
import React from 'react';
import { getCurrentRole } from './api'; // Corrected import path assuming './api'
import ViewUserPage from './pages/user/view-user-page'; // Corrected import path

// PrivateRoute component to handle authentication and role-based access
const PrivateRoute: React.FC<{ adminOnly?: boolean }> = ({ adminOnly = false }) => {
  const { isAuthenticated } = useAuth();
  const role = getCurrentRole();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (adminOnly && role !== 'admin') {
    return <Navigate to="/home" replace />;
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
          <Route path="/companies/edit/:companyId" element={<ManageCompanyPage />} />
          <Route path="/companies" element={<CompaniesPage />} />
          <Route path="/relationships" element={<RelationshipsPage />} />
          {isAuthenticated ? (
            <>
              <Route path="/home" element={<HomePage />} />
              <Route path="*" element={<Navigate to="/home" replace />} />
            </>
          ) : (
            <Route path="*" element={<Navigate to="/login" replace />} />
          )}
        </Route>
        <Route element={<PrivateRoute adminOnly />}>
          <Route path="/user/:userId/manage" element={<ManageUserPage />} />
          <Route path="/user/add" element={<ManageUserPage />} />
          <Route path="/tenant/:tenantId/user/add" element={<ManageUserPage />} />
          <Route path="/tenants" element={<TenantsPage />} />
          <Route path="/tenant/add" element={<ManageTenantPage />} />
          <Route path="/tenant/:tenantId/manage" element={<ManageTenantPage />} />
          <Route path="/tenant/:tenantId" element={<ViewTenantPage />} />
        </Route>
      </Routes>
    </Router>
  );
};

export default AppRoutes;