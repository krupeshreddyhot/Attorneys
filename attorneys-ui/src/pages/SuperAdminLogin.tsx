import { useState } from "react";
import {
  Alert,
  Box,
  Button,
  Container,
  Paper,
  TextField,
  Typography,
  InputAdornment,
  IconButton,
} from "@mui/material";
import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";
import AdminPanelSettingsIcon from "@mui/icons-material/AdminPanelSettings";
import { Link as RouterLink, useNavigate } from "react-router-dom";
import axios from "axios";
import api from "../services/api";
import { useAuth } from "../context/AuthContext";

const SuperAdminLogin = () => {
  const [userName, setUserName] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
      const { data } = await api.post<{ token: string }>("/auth/super-admin-login", {
        userName,
        password,
      });
      login(data.token);
      navigate("/app/organizations", { replace: true });
    } catch (err) {
      if (axios.isAxiosError(err)) {
        if (!err.response) {
          setError("Cannot reach the API. Press F5 in Visual Studio to start Attorneys.API, then retry.");
        } else if (err.response.status >= 500) {
          setError("API is not running. Press F5 in Visual Studio to start Attorneys.API, then retry.");
        } else {
          setError(
            typeof err.response.data === "string"
              ? err.response.data
              : "Invalid username or password.",
          );
        }
      } else {
        setError("Invalid username or password.");
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box sx={{ minHeight: "80vh", display: "flex", alignItems: "center", bgcolor: "#f4f6f8", py: { xs: 3, sm: 0 } }}>
      <Container maxWidth="sm" sx={{ px: { xs: 2, sm: 3 } }}>
        <Paper elevation={3} sx={{ p: { xs: 2.5, sm: 4 } }}>
          <Box sx={{ textAlign: "center", mb: 3 }}>
            <AdminPanelSettingsIcon sx={{ fontSize: 48, color: "#c9a227" }} />
            <Typography variant="h5" sx={{ fontWeight: 700 }}>
              Super Admin
            </Typography>
            <Typography color="text.secondary">Platform administration</Typography>
          </Box>
          {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
          <Box component="form" onSubmit={handleSubmit}>
            <TextField
              label="Username"
              fullWidth
              margin="normal"
              value={userName}
              onChange={(e) => setUserName(e.target.value)}
              required
            />
            <TextField
              label="Password"
              type={showPassword ? "text" : "password"}
              fullWidth
              margin="normal"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              slotProps={{
                input: {
                  endAdornment: (
                    <InputAdornment position="end">
                      <IconButton onClick={() => setShowPassword((v) => !v)} edge="end">
                        {showPassword ? <VisibilityOff /> : <Visibility />}
                      </IconButton>
                    </InputAdornment>
                  ),
                },
              }}
            />
            <Button type="submit" fullWidth variant="contained" disabled={loading} sx={{ mt: 3, bgcolor: "#0d1b2a" }}>
              {loading ? "Signing in…" : "Login"}
            </Button>
          </Box>
          <Button component={RouterLink} to="/login" fullWidth sx={{ mt: 2 }}>
            Firm staff login
          </Button>
        </Paper>
      </Container>
    </Box>
  );
};

export default SuperAdminLogin;
