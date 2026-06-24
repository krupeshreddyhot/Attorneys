import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import {
  Alert,
  Box,
  Button,
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
} from "@mui/material";
import UploadFileIcon from "@mui/icons-material/UploadFile";
import DownloadIcon from "@mui/icons-material/Download";
import DeleteIcon from "@mui/icons-material/Delete";
import api from "../../services/api";
import type { CaseDocument } from "../../types/legal";
import { deleteDocument, fetchDocuments, uploadDocument } from "../../services/legalService";
import { useAuth } from "../../context/AuthContext";
import PageHeader from "../../components/layout/PageHeader";
import ScrollableTable from "../../components/layout/ScrollableTable";

const DocumentsPage = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const { isAdministrator } = useAuth();
  const [caseNo, setCaseNo] = useState(searchParams.get("caseNo") ?? "");
  const [filterCaseNo, setFilterCaseNo] = useState(searchParams.get("caseNo") ?? "");
  const [description, setDescription] = useState("");
  const [fileType, setFileType] = useState("");
  const [file, setFile] = useState<File | null>(null);
  const [docs, setDocs] = useState<CaseDocument[]>([]);
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");

  const load = (cn?: string) => {
    fetchDocuments(cn || undefined).then(setDocs).catch(() => setDocs([]));
  };

  useEffect(() => { load(filterCaseNo || undefined); }, [filterCaseNo]);

  const handleUpload = async () => {
    setError("");
    setMessage("");
    if (!caseNo.trim() || !file) {
      setError("Case number and file are required.");
      return;
    }
    const form = new FormData();
    form.append("caseNo", caseNo.trim());
    form.append("description", description);
    form.append("fileType", fileType);
    form.append("file", file);
    try {
      await uploadDocument(form);
      setMessage("Document uploaded.");
      setFile(null);
      setDescription("");
      setFilterCaseNo(caseNo.trim());
      setSearchParams({ caseNo: caseNo.trim() });
      load(caseNo.trim());
    } catch {
      setError("Upload failed. Check case exists and file type (PDF, DOC, DOCX, JPG, PNG, TXT).");
    }
  };

  const handleDownload = async (doc: CaseDocument) => {
    const response = await api.get(`/documents/${doc.fileId}/download`, { responseType: "blob" });
    const url = window.URL.createObjectURL(response.data);
    const anchor = document.createElement("a");
    anchor.href = url;
    anchor.download = doc.fileName;
    anchor.click();
    window.URL.revokeObjectURL(url);
  };

  const handleDelete = async (fileId: number) => {
    await deleteDocument(fileId);
    load(filterCaseNo || undefined);
  };

  return (
    <>
      <PageHeader title="Documents" />

      <Paper sx={{ p: { xs: 2, sm: 3 }, mb: 3 }}>
        <Typography variant="h6" gutterBottom>Upload</Typography>
        {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
        {message && <Alert severity="success" sx={{ mb: 2 }}>{message}</Alert>}
        <Stack spacing={2} sx={{ mb: 2 }}>
          <TextField label="Case No" fullWidth value={caseNo} onChange={(e) => setCaseNo(e.target.value)} />
          <TextField label="Description" fullWidth value={description} onChange={(e) => setDescription(e.target.value)} />
          <TextField label="File Type" select fullWidth value={fileType} onChange={(e) => setFileType(e.target.value)}>
            <MenuItem value="">Auto</MenuItem>
            <MenuItem value="Petition">Petition</MenuItem>
            <MenuItem value="Order">Order</MenuItem>
            <MenuItem value="Evidence">Evidence</MenuItem>
            <MenuItem value="Other">Other</MenuItem>
          </TextField>
          <Button variant="outlined" component="label" fullWidth>
            Choose File
            <input hidden type="file" onChange={(e) => setFile(e.target.files?.[0] ?? null)} />
          </Button>
          <Button variant="contained" startIcon={<UploadFileIcon />} onClick={handleUpload} sx={{ bgcolor: "#0d1b2a" }} fullWidth>
            Upload
          </Button>
        </Stack>
        {file && <Typography variant="body2">Selected: {file.name}</Typography>}
      </Paper>

      <Box sx={{ mb: 2 }}>
        <TextField
          label="Filter by Case No"
          fullWidth
          value={filterCaseNo}
          onChange={(e) => {
            setFilterCaseNo(e.target.value);
            if (e.target.value) setSearchParams({ caseNo: e.target.value });
            else setSearchParams({});
          }}
        />
      </Box>

      <ScrollableTable>
        <Table size="small" sx={{ minWidth: 560 }}>
          <TableHead>
            <TableRow>
              <TableCell>Case No</TableCell>
              <TableCell>File</TableCell>
              <TableCell sx={{ display: { xs: "none", sm: "table-cell" } }}>Type</TableCell>
              <TableCell sx={{ display: { xs: "none", md: "table-cell" } }}>Description</TableCell>
              <TableCell sx={{ display: { xs: "none", sm: "table-cell" } }}>Uploaded</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {docs.map((doc) => (
              <TableRow key={doc.fileId}>
                <TableCell sx={{ fontWeight: 600 }}>{doc.caseNo}</TableCell>
                <TableCell sx={{ maxWidth: 140, overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }}>
                  {doc.fileName}
                </TableCell>
                <TableCell sx={{ display: { xs: "none", sm: "table-cell" } }}>{doc.fileType}</TableCell>
                <TableCell sx={{ display: { xs: "none", md: "table-cell" } }}>{doc.description}</TableCell>
                <TableCell sx={{ display: { xs: "none", sm: "table-cell" } }}>
                  {new Date(doc.uploadedAtUtc).toLocaleString()}
                </TableCell>
                <TableCell align="right" sx={{ whiteSpace: "nowrap" }}>
                  <IconButton aria-label="Download" onClick={() => handleDownload(doc)}><DownloadIcon /></IconButton>
                  {isAdministrator && (
                    <IconButton aria-label="Delete" onClick={() => handleDelete(doc.fileId)}><DeleteIcon /></IconButton>
                  )}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </ScrollableTable>
    </>
  );
};

export default DocumentsPage;
