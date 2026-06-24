import { useEffect, useState } from "react";
import {
  Alert,
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";
import api from "../../services/api";
import PageHeader from "../../components/layout/PageHeader";
import ScrollableTable from "../../components/layout/ScrollableTable";

type Firm = {
  id: number;
  name: string;
  code: string;
  isActive: boolean;
  createdAtUtc: string;
  adminUserName?: string;
};

const OrganizationsPage = () => {
  const [firms, setFirms] = useState<Firm[]>([]);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [firmName, setFirmName] = useState("");
  const [firmCode, setFirmCode] = useState("");
  const [adminUserName, setAdminUserName] = useState("");
  const [adminPassword, setAdminPassword] = useState("");
  const [saving, setSaving] = useState(false);
  const [resetFirm, setResetFirm] = useState<Firm | null>(null);
  const [resetPassword, setResetPassword] = useState("");
  const [resetting, setResetting] = useState(false);

  const loadFirms = () =>
    api
      .get<Firm[]>("/organizations/firms")
      .then((r: { data: Firm[] }) => setFirms(r.data))
      .catch(() => setError("Could not load firms."));

  useEffect(() => {
    loadFirms();
  }, []);

  const handleProvision = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setSuccess("");
    setSaving(true);
    try {
      const res = await api.post<{ firmCode: string; adminUserName: string }>("/organizations/firms", {
        firmName,
        firmCode,
        adminUserName,
        adminPassword,
      });
      setSuccess(
        `Firm ${res.data.firmCode} created. Admin login: firm code ${res.data.firmCode}, username ${res.data.adminUserName}, and the password you entered.`,
      );
      setFirmName("");
      setFirmCode("");
      setAdminUserName("");
      setAdminPassword("");
      loadFirms();
    } catch {
      setError("Could not create firm. Check code is unique and fields are valid.");
    } finally {
      setSaving(false);
    }
  };

  const handleResetPassword = async () => {
    if (!resetFirm || !resetPassword) return;
    setResetting(true);
    setError("");
    setSuccess("");
    try {
      const res = await api.post<{ code: string; userName: string }>(
        `/organizations/firms/${resetFirm.code}/reset-admin-password`,
        { newPassword: resetPassword },
      );
      setSuccess(`Password reset for ${res.data.code} admin "${res.data.userName}".`);
      setResetFirm(null);
      setResetPassword("");
    } catch {
      setError(`Could not reset password for ${resetFirm.code}.`);
    } finally {
      setResetting(false);
    }
  };

  return (
    <>
      <PageHeader title="Organizations" />

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      {success && <Alert severity="success" sx={{ mb: 2 }}>{success}</Alert>}

      <Paper sx={{ p: { xs: 2, sm: 3 }, mb: 3 }}>
        <Typography variant="h6" gutterBottom>
          Provision New Law Firm
        </Typography>
        <Box component="form" onSubmit={handleProvision}>
          <Stack spacing={2}>
            <TextField label="Firm Name" value={firmName} onChange={(e) => setFirmName(e.target.value)} required />
            <TextField
              label="Firm Code"
              value={firmCode}
              onChange={(e) => setFirmCode(e.target.value.toUpperCase())}
              required
              helperText="Public URL will be /{code} and used at login"
            />
            <TextField label="Admin Username" value={adminUserName} onChange={(e) => setAdminUserName(e.target.value)} required />
            <TextField
              label="Admin Password"
              type="password"
              value={adminPassword}
              onChange={(e) => setAdminPassword(e.target.value)}
              required
            />
            <Button type="submit" variant="contained" disabled={saving} sx={{ bgcolor: "#0d1b2a", alignSelf: "flex-start" }}>
              {saving ? "Creating…" : "Create Firm"}
            </Button>
          </Stack>
        </Box>
      </Paper>

      <Paper sx={{ p: { xs: 2, sm: 3 } }}>
        <Typography variant="h6" gutterBottom>
          All Firms
        </Typography>
        <ScrollableTable noPaper>
          <Table size="small" sx={{ minWidth: 640 }}>
          <TableHead>
            <TableRow>
              <TableCell>Code</TableCell>
              <TableCell>Name</TableCell>
              <TableCell>Admin Username</TableCell>
              <TableCell>Active</TableCell>
              <TableCell>Created</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {firms.map((f) => (
              <TableRow key={f.id}>
                <TableCell>{f.code}</TableCell>
                <TableCell>{f.name}</TableCell>
                <TableCell>{f.adminUserName ?? "—"}</TableCell>
                <TableCell>{f.isActive ? "Yes" : "No"}</TableCell>
                <TableCell>{new Date(f.createdAtUtc).toLocaleDateString()}</TableCell>
                <TableCell align="right">
                  {f.adminUserName && (
                    <Button size="small" onClick={() => setResetFirm(f)}>
                      Reset admin password
                    </Button>
                  )}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
        </ScrollableTable>
      </Paper>

      <Dialog open={resetFirm !== null} onClose={() => setResetFirm(null)} fullWidth maxWidth="xs">
        <DialogTitle>Reset admin password — {resetFirm?.code}</DialogTitle>
        <DialogContent>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            Administrator: {resetFirm?.adminUserName}
          </Typography>
          <TextField
            label="New password"
            type="password"
            fullWidth
            value={resetPassword}
            onChange={(e) => setResetPassword(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setResetFirm(null)}>Cancel</Button>
          <Button variant="contained" disabled={resetting || !resetPassword} onClick={handleResetPassword}>
            {resetting ? "Saving…" : "Reset password"}
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};

export default OrganizationsPage;
