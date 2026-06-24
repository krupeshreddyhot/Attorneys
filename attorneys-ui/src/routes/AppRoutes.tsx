import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";

import PublicLayout from "../layouts/PublicLayout";
import AppLayout from "../layouts/AppLayout";
import ProtectedRoute from "./ProtectedRoute";

import LandingPage from "../pages/landing/LandingPage";
import FirmLandingPage from "../pages/landing/FirmLandingPage";
import Login from "../pages/Login";
import SuperAdminLogin from "../pages/SuperAdminLogin";
import Dashboard from "../pages/app/Dashboard";
import CourtsPage from "../pages/app/CourtsPage";
import CasesPage from "../pages/app/CasesPage";
import CaseEntryPage from "../pages/app/CaseEntryPage";
import ReportsPage from "../pages/app/ReportsPage";
import DocumentsPage from "../pages/app/DocumentsPage";
import AccountsPage from "../pages/app/AccountsPage";
import OrganizationsPage from "../pages/app/OrganizationsPage";
import WebsitePage from "../pages/app/WebsitePage";

const AppRoutes = () => (
  <BrowserRouter>
    <Routes>
      <Route element={<PublicLayout />}>
        <Route path="/" element={<LandingPage />} />
        <Route path="/login" element={<Login />} />
        <Route path="/super-admin-login" element={<SuperAdminLogin />} />
        <Route path="/:firmCode" element={<FirmLandingPage />} />
      </Route>

      <Route
        path="/app"
        element={
          <ProtectedRoute>
            <AppLayout />
          </ProtectedRoute>
        }
      >
        <Route index element={<Navigate to="dashboard" replace />} />
        <Route path="dashboard" element={<Dashboard />} />
        <Route path="courts" element={<CourtsPage />} />
        <Route path="cases" element={<CasesPage />} />
        <Route path="cases/new" element={<CaseEntryPage />} />
        <Route path="cases/:caseNo/edit" element={<CaseEntryPage />} />
        <Route path="reports" element={<ReportsPage />} />
        <Route path="documents" element={<DocumentsPage />} />
        <Route
          path="website"
          element={
            <ProtectedRoute adminOnly>
              <WebsitePage />
            </ProtectedRoute>
          }
        />
        <Route
          path="accounts"
          element={
            <ProtectedRoute adminOnly>
              <AccountsPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="organizations"
          element={
            <ProtectedRoute superAdminOnly>
              <OrganizationsPage />
            </ProtectedRoute>
          }
        />
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  </BrowserRouter>
);

export default AppRoutes;
