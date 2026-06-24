import { useEffect, useState } from "react";
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
} from "@mui/material";
import api from "../../services/api";
import { useAuth } from "../../context/AuthContext";
import PageHeader from "../../components/layout/PageHeader";
import ScrollableTable from "../../components/layout/ScrollableTable";

type Court = { courtId: string; courtName: string; courtCity?: string };

const CourtsPage = () => {
  const { isAdministrator } = useAuth();
  const [courts, setCourts] = useState<Court[]>([]);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ courtId: "", courtName: "", courtCity: "" });

  const load = () => api.get<Court[]>("/courts").then((res) => setCourts(res.data));

  useEffect(() => { load(); }, []);

  const handleSave = async () => {
    await api.post("/courts", form);
    setOpen(false);
    setForm({ courtId: "", courtName: "", courtCity: "" });
    load();
  };

  return (
    <>
      <PageHeader
        title="Courts"
        action={
          isAdministrator ? (
            <Button variant="contained" onClick={() => setOpen(true)} sx={{ bgcolor: "#0d1b2a", alignSelf: { xs: "stretch", sm: "auto" } }} fullWidth>
              Add Court
            </Button>
          ) : undefined
        }
      />

      <ScrollableTable>
        <Table size="small" sx={{ minWidth: 480 }}>
          <TableHead>
            <TableRow>
              <TableCell>ID</TableCell>
              <TableCell>Name</TableCell>
              <TableCell>City</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {courts.map((c) => (
              <TableRow key={c.courtId}>
                <TableCell>{c.courtId}</TableCell>
                <TableCell>{c.courtName}</TableCell>
                <TableCell>{c.courtCity}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </ScrollableTable>

      <Dialog open={open} onClose={() => setOpen(false)} fullWidth maxWidth="sm" fullScreen={false}>
        <DialogTitle>Add Court</DialogTitle>
        <DialogContent>
          <TextField label="Court ID" fullWidth margin="dense" value={form.courtId} onChange={(e) => setForm({ ...form, courtId: e.target.value })} />
          <TextField label="Court Name" fullWidth margin="dense" value={form.courtName} onChange={(e) => setForm({ ...form, courtName: e.target.value })} />
          <TextField label="City" fullWidth margin="dense" value={form.courtCity} onChange={(e) => setForm({ ...form, courtCity: e.target.value })} />
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 2 }}>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSave}>Save</Button>
        </DialogActions>
      </Dialog>
    </>
  );
};

export default CourtsPage;
