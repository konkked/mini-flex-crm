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
import CustomersPage from '@pages/customer/customers-page';

// PrivateRoute component to handle authentication and role-based access
const PrivateRoute: React.FC<{ adminOnly?: boolean; superAdminOnly?: boolean; }> = ({ adminOnly = false, superAdminOnly = false }) => {
  const { isAuthenticated } = useAuth();
  const role = getCurrentRole();
  const tenantId = getCurrentTenantId();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (adminOnly && role !== 'admin') {
    return <Navigate to="/home" replace />;
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
          <Route path="/companies" element={<CompaniesPage />} />
          <Route path="/customers" element={<CustomersPage />} />
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
          <Route path="/company/:companyId/manage" element={<ManageCompanyPage />} />
          <Route path="/company/add" element={<ManageCompanyPage />} />
        </Route>
        <Route element={<PrivateRoute superAdminOnly />}>
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