import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  Grid,
  IconButton,
  MenuItem,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography,
  useMediaQuery,
  useTheme,
} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import DeleteIcon from "@mui/icons-material/Delete";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import type { CaseDetailRow, CasePayload, CaseType, Court } from "../../types/legal";
import axios from "axios";
import { createCase, fetchCase, fetchCaseTypes, fetchCourts, updateCase } from "../../services/legalService";

const emptyDetail = (id: number): CaseDetailRow => ({
  caseNoId: id,
  stage: "",
  previousDate: "",
  nextDate: "",
  ia: "",
  iaStage: "",
});

const toDateInput = (value?: string | null) => (value ? value.substring(0, 10) : "");

const CaseEntryPage = () => {
  const { caseNo: editCaseNo } = useParams();
  const isEdit = Boolean(editCaseNo);
  const navigate = useNavigate();
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("md"));

  const [courts, setCourts] = useState<Court[]>([]);
  const [caseTypes, setCaseTypes] = useState<CaseType[]>([]);
  const [error, setError] = useState("");
  const [saving, setSaving] = useState(false);

  const [caseNo, setCaseNo] = useState("");
  const [courtId, setCourtId] = useState("");
  const [caseTypeId, setCaseTypeId] = useState("");
  const [appearingFor, setAppearingFor] = useState("");
  const [clientAddress, setClientAddress] = useState("");
  const [clientPhone, setClientPhone] = useState("");
  const [serialNo, setSerialNo] = useState("");
  const [dateOfFiling, setDateOfFiling] = useState("");
  const [dateOfAppearance, setDateOfAppearance] = useState("");
  const [otherParty, setOtherParty] = useState("");
  const [counselForOtherParty, setCounselForOtherParty] = useState("");
  const [remarks, setRemarks] = useState("");
  const [details, setDetails] = useState<CaseDetailRow[]>([emptyDetail(1)]);

  useEffect(() => {
    Promise.all([fetchCourts(), fetchCaseTypes()]).then(([c, t]) => {
      setCourts(c);
      setCaseTypes(t);
    });
  }, []);

  useEffect(() => {
    if (!editCaseNo) return;
    fetchCase(editCaseNo)
      .then((c) => {
        setCaseNo(c.caseNo);
        setCourtId(c.courtId ?? "");
        setCaseTypeId(c.caseTypeId ?? "");
        setAppearingFor(c.appearingFor ?? "");
        setClientAddress(c.clientAddress ?? "");
        setClientPhone(c.clientPhone ?? "");
        setSerialNo(c.serialNo ?? "");
        setDateOfFiling(toDateInput(c.dateOfFiling));
        setDateOfAppearance(toDateInput(c.dateOfAppearance));
        setOtherParty(c.otherParty ?? "");
        setCounselForOtherParty(c.counselForOtherParty ?? "");
        setRemarks(c.remarks ?? "");
        setDetails(
          c.details?.length
            ? c.details.map((d) => ({
                id: d.id,
                caseNoId: d.caseNoId,
                stage: d.stage ?? "",
                previousDate: toDateInput(d.previousDate),
                nextDate: toDateInput(d.nextDate),
                ia: d.ia ?? "",
                iaStage: d.iaStage ?? "",
              }))
            : [emptyDetail(1)],
        );
      })
      .catch(() => setError("Could not load case."));
  }, [editCaseNo]);

  const updateDetail = (index: number, field: keyof CaseDetailRow, value: string) => {
    setDetails((rows) => rows.map((row, i) => (i === index ? { ...row, [field]: value } : row)));
  };

  const addDetailRow = () => {
    setDetails((rows) => [...rows, emptyDetail(rows.length + 1)]);
  };

  const removeDetailRow = (index: number) => {
    setDetails((rows) => (rows.length <= 1 ? rows : rows.filter((_, i) => i !== index)));
  };

  const buildPayload = (): CasePayload => ({
    caseNo: isEdit ? editCaseNo! : caseNo.trim(),
    courtId: courtId || undefined,
    caseTypeId: caseTypeId || undefined,
    appearingFor: appearingFor || undefined,
    clientAddress: clientAddress || undefined,
    clientPhone: clientPhone || undefined,
    serialNo: serialNo || undefined,
    dateOfFiling: dateOfFiling || null,
    dateOfAppearance: dateOfAppearance || null,
    otherParty: otherParty || undefined,
    counselForOtherParty: counselForOtherParty || undefined,
    remarks: remarks || undefined,
    details: details.map((d, index) => ({
      caseNoId: d.caseNoId || index + 1,
      stage: d.stage || undefined,
      previousDate: d.previousDate || null,
      nextDate: d.nextDate || null,
      ia: d.ia || undefined,
      iaStage: d.iaStage || undefined,
    })),
  });

  const handleSave = async () => {
    setError("");
    if (!isEdit && !caseNo.trim()) {
      setError("Case number is required.");
      return;
    }
    setSaving(true);
    try {
      const payload = buildPayload();
      if (isEdit) await updateCase(editCaseNo!, payload);
      else await createCase(payload);
      navigate("/app/cases");
    } catch (err) {
      if (axios.isAxiosError(err)) {
        const data = err.response?.data;
        let message = err.message;
        if (typeof data === "string") {
          message = data.length > 300 ? data.substring(0, 300) + "…" : data;
        } else if (data && typeof data === "object") {
          const obj = data as { title?: string; detail?: string; errors?: Record<string, string[]> };
          if (obj.title) message = obj.title;
          else if (obj.detail) message = obj.detail;
          else if (obj.errors) {
            message = Object.values(obj.errors).flat().join(" ");
          }
        }
        setError(message || "Could not save case.");
      } else {
        setError("Could not save case. Check case number is unique and fields are valid.");
      }
    } finally {
      setSaving(false);
    }
  };

  return (
    <>
      <Stack direction="row" spacing={1} sx={{ mb: 2, alignItems: "center" }}>
        <IconButton onClick={() => navigate("/app/cases")} aria-label="Back to cases">
          <ArrowBackIcon />
        </IconButton>
        <Typography variant="h4" sx={{ fontWeight: 700, fontSize: { xs: "1.35rem", sm: "2.125rem" }, lineHeight: 1.2 }}>
          {isEdit ? `Edit — ${editCaseNo}` : "New Case Entry"}
        </Typography>
      </Stack>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <Paper sx={{ p: { xs: 2, sm: 3 }, mb: 3 }}>
        <Typography variant="h6" gutterBottom>Case Information</Typography>
        <Grid container spacing={2}>
          <Grid size={{ xs: 12, md: 4 }}>
            <TextField
              label="Case No"
              fullWidth
              required
              disabled={isEdit}
              value={isEdit ? editCaseNo : caseNo}
              onChange={(e) => setCaseNo(e.target.value)}
            />
          </Grid>
          <Grid size={{ xs: 12, md: 4 }}>
            <TextField select label="Court" fullWidth value={courtId} onChange={(e) => setCourtId(e.target.value)}>
              <MenuItem value="">— Select —</MenuItem>
              {courts.map((c) => (
                <MenuItem key={c.courtId} value={c.courtId}>{c.courtName}</MenuItem>
              ))}
            </TextField>
          </Grid>
          <Grid size={{ xs: 12, md: 4 }}>
            <TextField select label="Type of Case" fullWidth value={caseTypeId} onChange={(e) => setCaseTypeId(e.target.value)}>
              <MenuItem value="">— Select —</MenuItem>
              {caseTypes.map((t) => (
                <MenuItem key={t.caseTypeId} value={t.caseTypeId}>{t.name}</MenuItem>
              ))}
            </TextField>
          </Grid>
          <Grid size={{ xs: 12, md: 6 }}>
            <TextField label="Appearing For" fullWidth value={appearingFor} onChange={(e) => setAppearingFor(e.target.value)} />
          </Grid>
          <Grid size={{ xs: 12, md: 6 }}>
            <TextField label="Serial No" fullWidth value={serialNo} onChange={(e) => setSerialNo(e.target.value)} />
          </Grid>
          <Grid size={{ xs: 12, md: 6 }}>
            <TextField label="Client Address" fullWidth multiline minRows={2} value={clientAddress} onChange={(e) => setClientAddress(e.target.value)} />
          </Grid>
          <Grid size={{ xs: 12, md: 6 }}>
            <TextField label="Client Phone" fullWidth value={clientPhone} onChange={(e) => setClientPhone(e.target.value)} />
          </Grid>
          <Grid size={{ xs: 12, md: 3 }}>
            <TextField label="Date of Filing" type="date" fullWidth slotProps={{ inputLabel: { shrink: true } }} value={dateOfFiling} onChange={(e) => setDateOfFiling(e.target.value)} />
          </Grid>
          <Grid size={{ xs: 12, md: 3 }}>
            <TextField label="Date of Appearance" type="date" fullWidth slotProps={{ inputLabel: { shrink: true } }} value={dateOfAppearance} onChange={(e) => setDateOfAppearance(e.target.value)} />
          </Grid>
          <Grid size={{ xs: 12, md: 6 }}>
            <TextField label="Other Party" fullWidth value={otherParty} onChange={(e) => setOtherParty(e.target.value)} />
          </Grid>
          <Grid size={{ xs: 12, md: 6 }}>
            <TextField label="Counsel for Other Party" fullWidth value={counselForOtherParty} onChange={(e) => setCounselForOtherParty(e.target.value)} />
          </Grid>
          <Grid size={{ xs: 12 }}>
            <TextField label="Remarks" fullWidth multiline minRows={2} value={remarks} onChange={(e) => setRemarks(e.target.value)} />
          </Grid>
        </Grid>
      </Paper>

      <Paper sx={{ p: { xs: 2, sm: 3 }, mb: 3 }}>
        <Box sx={{ display: "flex", flexDirection: { xs: "column", sm: "row" }, justifyContent: "space-between", gap: 2, mb: 2 }}>
          <Typography variant="h6">Hearing Details</Typography>
          <Button startIcon={<AddIcon />} onClick={addDetailRow} fullWidth={isMobile}>
            Add Row
          </Button>
        </Box>

        {isMobile ? (
          <Stack spacing={2}>
            {details.map((row, index) => (
              <Card key={index} variant="outlined">
                <CardContent>
                  <Stack spacing={2}>
                    <TextField label="Stage" fullWidth value={row.stage} onChange={(e) => updateDetail(index, "stage", e.target.value)} />
                    <TextField label="Date" type="date" fullWidth slotProps={{ inputLabel: { shrink: true } }} value={row.previousDate} onChange={(e) => updateDetail(index, "previousDate", e.target.value)} />
                    <TextField label="Next Date" type="date" fullWidth slotProps={{ inputLabel: { shrink: true } }} value={row.nextDate} onChange={(e) => updateDetail(index, "nextDate", e.target.value)} />
                    <TextField label="IA" fullWidth value={row.ia} onChange={(e) => updateDetail(index, "ia", e.target.value)} />
                    <TextField label="IA Stage" fullWidth value={row.iaStage} onChange={(e) => updateDetail(index, "iaStage", e.target.value)} />
                    <Button color="error" startIcon={<DeleteIcon />} onClick={() => removeDetailRow(index)}>
                      Remove hearing
                    </Button>
                  </Stack>
                </CardContent>
              </Card>
            ))}
          </Stack>
        ) : (
          <Box sx={{ overflowX: "auto" }}>
            <Table size="small" sx={{ minWidth: 720 }}>
              <TableHead>
                <TableRow>
                  <TableCell>Stage</TableCell>
                  <TableCell>Date</TableCell>
                  <TableCell>Next Date</TableCell>
                  <TableCell>IA</TableCell>
                  <TableCell>IA Stage</TableCell>
                  <TableCell width={60} />
                </TableRow>
              </TableHead>
              <TableBody>
                {details.map((row, index) => (
                  <TableRow key={index}>
                    <TableCell>
                      <TextField size="small" fullWidth value={row.stage} onChange={(e) => updateDetail(index, "stage", e.target.value)} />
                    </TableCell>
                    <TableCell>
                      <TextField size="small" type="date" fullWidth slotProps={{ inputLabel: { shrink: true } }} value={row.previousDate} onChange={(e) => updateDetail(index, "previousDate", e.target.value)} />
                    </TableCell>
                    <TableCell>
                      <TextField size="small" type="date" fullWidth slotProps={{ inputLabel: { shrink: true } }} value={row.nextDate} onChange={(e) => updateDetail(index, "nextDate", e.target.value)} />
                    </TableCell>
                    <TableCell>
                      <TextField size="small" fullWidth value={row.ia} onChange={(e) => updateDetail(index, "ia", e.target.value)} />
                    </TableCell>
                    <TableCell>
                      <TextField size="small" fullWidth value={row.iaStage} onChange={(e) => updateDetail(index, "iaStage", e.target.value)} />
                    </TableCell>
                    <TableCell>
                      <IconButton size="small" onClick={() => removeDetailRow(index)} aria-label="Remove hearing">
                        <DeleteIcon fontSize="small" />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </Box>
        )}
      </Paper>

      <Stack direction={{ xs: "column", sm: "row" }} spacing={2}>
        <Button variant="contained" onClick={handleSave} disabled={saving} sx={{ bgcolor: "#0d1b2a" }} fullWidth>
          {saving ? "Saving…" : "Save Case"}
        </Button>
        <Button onClick={() => navigate("/app/cases")} fullWidth>Cancel</Button>
        {isEdit && (
          <Button variant="outlined" fullWidth onClick={() => navigate(`/app/documents?caseNo=${encodeURIComponent(editCaseNo!)}`)}>
            Documents
          </Button>
        )}
      </Stack>
    </>
  );
};

export default CaseEntryPage;
