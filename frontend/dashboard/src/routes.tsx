import { BrowserRouter as Router, Routes, Route, Navigate, Outlet } from 'react-router-dom';
import LoginPage from './pages/auth/login-page';
import AddUserPage from './pages/user/manage-user-page';
import { useAuth } from './hooks/useAuth';
import UsersPage from './pages/user/users-page';
import EditCompanyPage from './pages/company/edit-company-page';
import CompaniesPage from './pages/company/companies-page';
import HomePage from './pages/home/home-page';
import RelationshipsPage from './pages/relation/relationships-page';
import TenantsPage from './pages/tenant/tenant-page';
import ManageTenantPage from './pages/tenant/manage-tenant-page';
import React from 'react';
import { getCurrentRole } from './api'; // Corrected import path assuming './api'

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
          <Route path="/companies/edit/:companyId" element={<EditCompanyPage />} />
          <Route path="/companies" element={<CompaniesPage />} />
          <Route path="/relationships" element={<RelationshipsPage />} />
          <Route path="/tenants" element={<TenantsPage />} />
          <Route path="/tenant" element={<ManageTenantPage />} />
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
          <Route path="/user/add" element={<AddUserPage />} />
          <Route path="/tenant/:tenantId/user/add" element={<AddUserPage />} />
        </Route>
      </Routes>
    </Router>
  );
};

export default AppRoutes;