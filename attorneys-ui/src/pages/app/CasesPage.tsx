import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Box,
  Button,
  IconButton,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import EditIcon from "@mui/icons-material/Edit";
import api from "../../services/api";
import PageHeader from "../../components/layout/PageHeader";
import ScrollableTable from "../../components/layout/ScrollableTable";

type CaseRow = {
  caseNo: string;
  courtName?: string;
  caseType?: string;
  appearingFor?: string;
  dateOfFiling?: string;
  detailCount: number;
};

const CasesPage = () => {
  const navigate = useNavigate();
  const [cases, setCases] = useState<CaseRow[]>([]);
  const [search, setSearch] = useState("");

  const load = (term?: string) => {
    api.get<CaseRow[]>("/cases", { params: term ? { search: term } : {} }).then((res) => setCases(res.data));
  };

  useEffect(() => { load(); }, []);

  return (
    <>
      <PageHeader
        title="Cases"
        action={
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() => navigate("/app/cases/new")}
            sx={{ bgcolor: "#0d1b2a", alignSelf: { xs: "stretch", sm: "auto" } }}
            fullWidth
          >
            New Case
          </Button>
        }
      />

      <Box sx={{ mb: 2 }}>
        <TextField
          label="Search cases"
          fullWidth
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          onKeyDown={(e) => e.key === "Enter" && load(search)}
        />
      </Box>

      <ScrollableTable>
        <Table size="small" sx={{ minWidth: 640 }}>
          <TableHead>
            <TableRow>
              <TableCell>Case No</TableCell>
              <TableCell>Court</TableCell>
              <TableCell>Type</TableCell>
              <TableCell sx={{ display: { xs: "none", sm: "table-cell" } }}>Client</TableCell>
              <TableCell sx={{ display: { xs: "none", md: "table-cell" } }}>Filing Date</TableCell>
              <TableCell>Hearings</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {cases.map((c) => (
              <TableRow key={c.caseNo} hover>
                <TableCell sx={{ fontWeight: 600 }}>{c.caseNo}</TableCell>
                <TableCell>{c.courtName}</TableCell>
                <TableCell>{c.caseType}</TableCell>
                <TableCell sx={{ display: { xs: "none", sm: "table-cell" } }}>{c.appearingFor}</TableCell>
                <TableCell sx={{ display: { xs: "none", md: "table-cell" } }}>
                  {c.dateOfFiling ? new Date(c.dateOfFiling).toLocaleDateString() : ""}
                </TableCell>
                <TableCell>{c.detailCount}</TableCell>
                <TableCell align="right">
                  <IconButton aria-label="Edit case" onClick={() => navigate(`/app/cases/${encodeURIComponent(c.caseNo)}/edit`)}>
                    <EditIcon />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </ScrollableTable>
    </>
  );
};

export default CasesPage;
