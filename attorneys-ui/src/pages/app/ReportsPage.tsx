import { useEffect, useState } from "react";
import {
  Box,
  MenuItem,
  Tab,
  Tabs,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
} from "@mui/material";
import type { Court, CourtWiseRow, ReportRow } from "../../types/legal";
import {
  fetchCourtWiseReport,
  fetchCourts,
  fetchDiaryReport,
  fetchPendingReport,
} from "../../services/legalService";
import PageHeader from "../../components/layout/PageHeader";
import ScrollableTable from "../../components/layout/ScrollableTable";

const fmt = (d?: string) => (d ? new Date(d).toLocaleDateString() : "");

const ReportsPage = () => {
  const [tab, setTab] = useState(0);
  const [courts, setCourts] = useState<Court[]>([]);
  const [courtId, setCourtId] = useState("");
  const [diaryDate, setDiaryDate] = useState(new Date().toISOString().substring(0, 10));
  const [todayRows, setTodayRows] = useState<ReportRow[]>([]);
  const [diaryRows, setDiaryRows] = useState<ReportRow[]>([]);
  const [courtRows, setCourtRows] = useState<CourtWiseRow[]>([]);
  const [pendingRows, setPendingRows] = useState<ReportRow[]>([]);

  useEffect(() => {
    fetchCourts().then(setCourts);
    fetchDiaryReport().then((r) => setTodayRows(r.rows));
    fetchPendingReport().then(setPendingRows);
  }, []);

  useEffect(() => {
    fetchDiaryReport(diaryDate).then((r) => setDiaryRows(r.rows));
  }, [diaryDate]);

  useEffect(() => {
    if (courtId) fetchCourtWiseReport(courtId).then(setCourtRows);
    else setCourtRows([]);
  }, [courtId]);

  const reportTable = (rows: ReportRow[], showStage = true) => (
    <ScrollableTable>
      <Table size="small" sx={{ minWidth: 640 }}>
        <TableHead>
          <TableRow>
            <TableCell>Case No</TableCell>
            <TableCell>Court</TableCell>
            <TableCell sx={{ display: { xs: "none", sm: "table-cell" } }}>Type</TableCell>
            <TableCell>Client</TableCell>
            {showStage && <TableCell sx={{ display: { xs: "none", md: "table-cell" } }}>Stage</TableCell>}
            <TableCell>Next Date</TableCell>
            <TableCell sx={{ display: { xs: "none", sm: "table-cell" } }}>IA</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {rows.length === 0 ? (
            <TableRow><TableCell colSpan={7} align="center">No records</TableCell></TableRow>
          ) : (
            rows.map((r, i) => (
              <TableRow key={`${r.caseNo}-${i}`}>
                <TableCell sx={{ fontWeight: 600 }}>{r.caseNo}</TableCell>
                <TableCell>{r.courtName}</TableCell>
                <TableCell sx={{ display: { xs: "none", sm: "table-cell" } }}>{r.caseType}</TableCell>
                <TableCell>{r.appearingFor}</TableCell>
                {showStage && <TableCell sx={{ display: { xs: "none", md: "table-cell" } }}>{r.stage}</TableCell>}
                <TableCell>{fmt(r.nextDate)}</TableCell>
                <TableCell sx={{ display: { xs: "none", sm: "table-cell" } }}>{r.ia}</TableCell>
              </TableRow>
            ))
          )}
        </TableBody>
      </Table>
    </ScrollableTable>
  );

  return (
    <>
      <PageHeader title="Reports" />
      <Tabs
        value={tab}
        onChange={(_, v) => setTab(v)}
        variant="scrollable"
        scrollButtons="auto"
        allowScrollButtonsMobile
        sx={{ mb: 2 }}
      >
        <Tab label="Today" />
        <Tab label="Diary" />
        <Tab label="Court Wise" />
        <Tab label="Pending" />
      </Tabs>

      {tab === 0 && reportTable(todayRows)}

      {tab === 1 && (
        <Box>
          <TextField
            label="Diary Date"
            type="date"
            fullWidth
            slotProps={{ inputLabel: { shrink: true } }}
            value={diaryDate}
            onChange={(e) => setDiaryDate(e.target.value)}
            sx={{ mb: 2, maxWidth: { sm: 280 } }}
          />
          {reportTable(diaryRows)}
        </Box>
      )}

      {tab === 2 && (
        <Box>
          <TextField
            select
            label="Court"
            fullWidth
            value={courtId}
            onChange={(e) => setCourtId(e.target.value)}
            sx={{ mb: 2, maxWidth: { sm: 400 } }}
          >
            <MenuItem value="">— Select court —</MenuItem>
            {courts.map((c) => (
              <MenuItem key={c.courtId} value={c.courtId}>{c.courtName}</MenuItem>
            ))}
          </TextField>
          <ScrollableTable>
            <Table size="small" sx={{ minWidth: 700 }}>
              <TableHead>
                <TableRow>
                  <TableCell>Case No</TableCell>
                  <TableCell>Court</TableCell>
                  <TableCell sx={{ display: { xs: "none", sm: "table-cell" } }}>Type</TableCell>
                  <TableCell>Client</TableCell>
                  <TableCell sx={{ display: { xs: "none", md: "table-cell" } }}>Filing Date</TableCell>
                  <TableCell>Next Date</TableCell>
                  <TableCell>Hearings</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {courtRows.length === 0 ? (
                  <TableRow><TableCell colSpan={7} align="center">Select a court</TableCell></TableRow>
                ) : (
                  courtRows.map((r) => (
                    <TableRow key={r.caseNo}>
                      <TableCell sx={{ fontWeight: 600 }}>{r.caseNo}</TableCell>
                      <TableCell>{r.courtName}</TableCell>
                      <TableCell sx={{ display: { xs: "none", sm: "table-cell" } }}>{r.caseType}</TableCell>
                      <TableCell>{r.appearingFor}</TableCell>
                      <TableCell sx={{ display: { xs: "none", md: "table-cell" } }}>{fmt(r.dateOfFiling)}</TableCell>
                      <TableCell>{fmt(r.latestNextDate)}</TableCell>
                      <TableCell>{r.hearingCount}</TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </ScrollableTable>
        </Box>
      )}

      {tab === 3 && reportTable(pendingRows)}
    </>
  );
};

export default ReportsPage;
