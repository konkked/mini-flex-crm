import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import NavbarComponent from './components/navbar/navbar-component';
import LoginPage from './pages/auth/login-page';
import SignupPage from './pages/auth/signup-page';
import { useAuth } from './hooks/useAuth';
import UsersPage from './pages/user/users-page';
import EditCompanyPage from './pages/company/edit-company-page';
import CompaniesPage from './pages/company/companies-page';
import HomePage from './pages/home/home-page';
import RelationshipsPage from './pages/relation/relationships-page';
import TenantsPage from './pages/tenant/tenant-page';
import CreateTenantPage from './pages/tenant/create-tenant-page';
import React from 'react';

const AppRoutes = () => {
  const { isAuthenticated } = useAuth();

  return (
    <Router>
      <Routes>
        <Route path="/users" element={<UsersPage/>} />
        <Route path="/companies/edit/:companyId" element={<EditCompanyPage/>} />
        <Route path="/companies" element={<CompaniesPage/>} />
        <Route path="/signup" element={<SignupPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/relationships" element={<RelationshipsPage />} />
        <Route path="/tenants" element={<TenantsPage /> } />
        <Route path="/tenant" element={<CreateTenantPage />} />
        {isAuthenticated ? (
          <>
            <Route path="/home" element={<HomePage />} />
            <Route path="*" element={<Navigate to="/home" />} />
          </>
        ) : (
          <Route path="*" element={<Navigate to="/login" />} />
        )}
      </Routes>
    </Router>
  );
};

export default AppRoutes;
