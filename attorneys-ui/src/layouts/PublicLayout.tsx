import {
  AppBar,
  Box,
  Button,
  Container,
  Divider,
  Drawer,
  IconButton,
  Link,
  List,
  ListItem,
  ListItemButton,
  Toolbar,
  Typography,
} from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import GavelIcon from "@mui/icons-material/Gavel";
import MenuIcon from "@mui/icons-material/Menu";
import { Link as RouterLink, Outlet } from "react-router-dom";
import { useState } from "react";
import { FirmLandingProvider, useFirmLanding } from "../context/FirmLandingContext";

const PublicLayoutInner = () => {
  const { firmCode, landing } = useFirmLanding();
  const [menuOpen, setMenuOpen] = useState(false);
  const basePath = firmCode ? `/${firmCode}` : "/";
  const firmName = landing?.name ?? (firmCode ? firmCode : "Attorneys Chamber");
  const loginPath = firmCode ? `/login?firm=${firmCode}` : "/login";

  const navLinks = firmCode
    ? [
        { label: "Home", to: basePath },
        { label: "About", to: `${basePath}#about` },
        { label: "Practice Areas", to: `${basePath}#practice` },
        { label: "Advocates", to: `${basePath}#advocates` },
        { label: "Contact", to: `${basePath}#contact` },
      ]
    : [{ label: "Home", to: "/" }];

  return (
    <Box sx={{ display: "flex", flexDirection: "column", minHeight: "100dvh", width: "100%" }}>
      <AppBar position="sticky" sx={{ bgcolor: "#0d1b2a" }}>
        <Container maxWidth={false} sx={{ px: { xs: 2, sm: 3, md: 4 } }}>
          <Toolbar disableGutters sx={{ justifyContent: "space-between", gap: 1, minHeight: { xs: 56, sm: 64 } }}>
            <Box sx={{ display: "flex", alignItems: "center", gap: 0.5, minWidth: 0 }}>
              <IconButton
                color="inherit"
                edge="start"
                aria-label="Open menu"
                onClick={() => setMenuOpen(true)}
                sx={{ display: { md: "none" }, flexShrink: 0 }}
              >
                <MenuIcon />
              </IconButton>
              <GavelIcon sx={{ color: "#c9a227", flexShrink: 0 }} />
              <Box sx={{ minWidth: 0, ml: 0.5 }}>
                <Typography
                  variant="h6"
                  sx={{ fontWeight: 700, lineHeight: 1.2, fontSize: { xs: "1rem", sm: "1.25rem" } }}
                  noWrap
                >
                  {firmName}
                </Typography>
                {landing?.addressLine && (
                  <Typography variant="caption" sx={{ opacity: 0.8, display: { xs: "none", sm: "block" } }} noWrap>
                    {landing.addressLine}
                    {landing.city ? ` · ${landing.city}` : ""}
                  </Typography>
                )}
              </Box>
            </Box>

            <Box sx={{ display: { xs: "none", md: "flex" }, gap: 2, alignItems: "center", flexShrink: 0 }}>
              {navLinks.map((link) => (
                <Link key={link.to} component={RouterLink} to={link.to} color="inherit" underline="hover">
                  {link.label}
                </Link>
              ))}
              <Button
                component={RouterLink}
                to={loginPath}
                variant="contained"
                sx={{ bgcolor: "#c9a227", color: "#0d1b2a", "&:hover": { bgcolor: "#b8921f" } }}
              >
                Login
              </Button>
            </Box>

            <Button
              component={RouterLink}
              to={loginPath}
              variant="contained"
              size="small"
              sx={{
                display: { xs: "inline-flex", md: "none" },
                bgcolor: "#c9a227",
                color: "#0d1b2a",
                flexShrink: 0,
                "&:hover": { bgcolor: "#b8921f" },
              }}
            >
              Login
            </Button>
          </Toolbar>
        </Container>
      </AppBar>

      <Drawer
        anchor="left"
        open={menuOpen}
        onClose={() => setMenuOpen(false)}
        ModalProps={{ keepMounted: true }}
        sx={{ display: { md: "none" } }}
        slotProps={{ paper: { sx: { width: 300 } } }}
      >
        <Toolbar sx={{ justifyContent: "space-between" }}>
          <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>
            Menu
          </Typography>
          <IconButton onClick={() => setMenuOpen(false)} aria-label="Close menu">
            <CloseIcon />
          </IconButton>
        </Toolbar>
        <Divider />
        <List>
          {navLinks.map((link) => (
            <ListItem key={link.to} disablePadding>
              <ListItemButton component={RouterLink} to={link.to} onClick={() => setMenuOpen(false)}>
                {link.label}
              </ListItemButton>
            </ListItem>
          ))}
        </List>
      </Drawer>

      <Box component="main" sx={{ flex: 1, width: "100%" }}>
        <Outlet />
      </Box>

      {!firmCode && (
        <Box component="footer" sx={{ bgcolor: "#0d1b2a", color: "#fff", py: 4, mt: 6 }}>
          <Container maxWidth={false} sx={{ px: { xs: 2, sm: 3, md: 4 } }}>
            <Typography variant="body2" color="rgba(255,255,255,0.7)">
              © {new Date().getFullYear()} {firmName}. Legal practice management.
            </Typography>
          </Container>
        </Box>
      )}
    </Box>
  );
};

const PublicLayout = () => (
  <FirmLandingProvider>
    <PublicLayoutInner />
  </FirmLandingProvider>
);

export default PublicLayout;
