import { useState } from "react";
import {
  Alert,
  Box,
  Button,
  Container,
  Link,
  Paper,
  TextField,
  Typography,
  InputAdornment,
  IconButton,
} from "@mui/material";
import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";
import GavelIcon from "@mui/icons-material/Gavel";
import { Link as RouterLink, useNavigate, useLocation, useSearchParams } from "react-router-dom";
import axios from "axios";
import api from "../services/api";
import { useAuth } from "../context/AuthContext";

const Login = () => {
  const [searchParams] = useSearchParams();
  const initialFirm = searchParams.get("firm")?.toUpperCase() ?? "DEMO";
  const [firmCode, setFirmCode] = useState(initialFirm);
  const [userName, setUserName] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const from = (location.state as { from?: string })?.from ?? "/app/dashboard";

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
      const { data } = await api.post<{ token: string }>("/auth/login", {
        firmCode,
        userName,
        password,
      });
      login(data.token);
      navigate(from, { replace: true });
    } catch (err) {
      if (axios.isAxiosError(err)) {
        if (!err.response || err.code === "ERR_NETWORK") {
          setError("Cannot reach the API. Press F5 in Visual Studio to start Attorneys.API, then retry.");
        } else if (err.response.status >= 500) {
          setError("API is not running. Press F5 in Visual Studio to start Attorneys.API, then retry.");
        } else {
          const data = err.response.data;
          const serverMessage =
            typeof data === "string"
              ? data
              : typeof data === "object" && data && "title" in data
                ? String((data as { title?: string }).title)
                : "";
          if (serverMessage.includes("Invalid firm code")) {
            setError(`Firm code "${firmCode}" was not found. Check the code or ask Super Admin to provision the firm.`);
          } else if (serverMessage.includes("Invalid username or password")) {
            setError(
              firmCode === "DEMO"
                ? "Invalid username or password. For DEMO use admin / Admin@123."
                : `Invalid username or password for firm ${firmCode}. Use the admin account created when this firm was provisioned, or ask Super Admin to reset the password.`,
            );
          } else {
            setError(serverMessage || "Login failed. Check firm code, username, and password.");
          }
        }
      } else {
        setError("Login failed. Check firm code, username, and password.");
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
            <GavelIcon sx={{ fontSize: 48, color: "#c9a227" }} />
            <Typography variant="h5" sx={{ fontWeight: 700 }}>
              Staff Login
            </Typography>
            <Typography color="text.secondary">Sign in to your law firm workspace</Typography>
          </Box>
          {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
          <Box component="form" onSubmit={handleSubmit}>
            <TextField
              label="Firm Code"
              fullWidth
              margin="normal"
              value={firmCode}
              onChange={(e) => setFirmCode(e.target.value.toUpperCase())}
              required
              helperText={
                firmCode === "DEMO"
                  ? "Dev demo firm — try username admin, password Admin@123"
                  : "Each firm has its own admin account (not the DEMO password unless Super Admin set it that way)"
              }
            />
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
          <Typography sx={{ mt: 2, textAlign: "center" }}>
            <Link component={RouterLink} to="/super-admin-login">
              Super Admin login
            </Link>
          </Typography>
          <Button component={RouterLink} to={firmCode ? `/${firmCode}` : "/"} fullWidth sx={{ mt: 1 }}>
            Back to home
          </Button>
        </Paper>
      </Container>
    </Box>
  );
};

export default Login;
