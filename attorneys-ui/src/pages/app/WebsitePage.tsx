import { useEffect, useState } from "react";
import {
  Alert,
  Avatar,
  Box,
  Button,
  Card,
  CardContent,
  CardMedia,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Grid,
  IconButton,
  Stack,
  Tab,
  Tabs,
  TextField,
  Typography,
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import {
  websiteService,
  type AdvocateItem,
  type BannerItem,
  type PracticeAreaItem,
  type WebsiteProfile,
} from "../../services/websiteService";
import { publicMediaUrl } from "../../utils/mediaUrl";
import { useAuth } from "../../context/AuthContext";
import PageHeader from "../../components/layout/PageHeader";

const emptyProfile = (): Omit<WebsiteProfile, "name" | "code"> => ({
  addressLine: "",
  city: "",
  phone: "",
  email: "",
  heroTagline: "",
  heroSubtitle: "",
  aboutTitle: "",
  aboutBody: "",
  aboutHighlightTitle: "",
  aboutHighlightBody: "",
});

const WebsitePage = () => {
  const { user } = useAuth();
  const [tab, setTab] = useState(0);
  const [message, setMessage] = useState("");
  const [profile, setProfile] = useState<Omit<WebsiteProfile, "name" | "code">>(emptyProfile());
  const [firmCode, setFirmCode] = useState("");
  const [banners, setBanners] = useState<BannerItem[]>([]);
  const [practiceAreas, setPracticeAreas] = useState<PracticeAreaItem[]>([]);
  const [advocates, setAdvocates] = useState<AdvocateItem[]>([]);
  const [bannerFile, setBannerFile] = useState<File | null>(null);
  const [bannerCaption, setBannerCaption] = useState("");
  const [areaDialog, setAreaDialog] = useState<{ open: boolean; item?: PracticeAreaItem }>({ open: false });
  const [advocateDialog, setAdvocateDialog] = useState<{ open: boolean; item?: AdvocateItem }>({ open: false });
  const [areaForm, setAreaForm] = useState({ title: "", description: "", sortOrder: 0 });
  const [advocateForm, setAdvocateForm] = useState({ fullName: "", designation: "", bio: "", sortOrder: 0 });
  const [advocatePhoto, setAdvocatePhoto] = useState<File | null>(null);

  const loadAll = async () => {
    const [profileRes, bannersRes, areasRes, advocatesRes] = await Promise.all([
      websiteService.getProfile(),
      websiteService.listBanners(),
      websiteService.listPracticeAreas(),
      websiteService.listAdvocates(),
    ]);
    setFirmCode(profileRes.data.code);
    setProfile({
      addressLine: profileRes.data.addressLine ?? "",
      city: profileRes.data.city ?? "",
      phone: profileRes.data.phone ?? "",
      email: profileRes.data.email ?? "",
      heroTagline: profileRes.data.heroTagline ?? "",
      heroSubtitle: profileRes.data.heroSubtitle ?? "",
      aboutTitle: profileRes.data.aboutTitle ?? "",
      aboutBody: profileRes.data.aboutBody ?? "",
      aboutHighlightTitle: profileRes.data.aboutHighlightTitle ?? "",
      aboutHighlightBody: profileRes.data.aboutHighlightBody ?? "",
    });
    setBanners(bannersRes.data);
    setPracticeAreas(areasRes.data);
    setAdvocates(advocatesRes.data);
  };

  useEffect(() => {
    loadAll().catch(() => setMessage("Failed to load website settings."));
  }, []);

  const saveProfile = async () => {
    await websiteService.updateProfile(profile);
    setMessage("Profile saved.");
  };

  const uploadBanner = async () => {
    if (!bannerFile) return;
    await websiteService.uploadBanner(bannerFile, bannerCaption, banners.length);
    setBannerFile(null);
    setBannerCaption("");
    await loadAll();
    setMessage("Banner uploaded.");
  };

  const openAreaDialog = (item?: PracticeAreaItem) => {
    setAreaForm(
      item
        ? { title: item.title, description: item.description ?? "", sortOrder: item.sortOrder }
        : { title: "", description: "", sortOrder: practiceAreas.length },
    );
    setAreaDialog({ open: true, item });
  };

  const saveArea = async () => {
    if (areaDialog.item) {
      await websiteService.updatePracticeArea(areaDialog.item.id, areaForm);
    } else {
      await websiteService.createPracticeArea(areaForm);
    }
    setAreaDialog({ open: false });
    await loadAll();
    setMessage("Practice area saved.");
  };

  const openAdvocateDialog = (item?: AdvocateItem) => {
    setAdvocateForm(
      item
        ? {
            fullName: item.fullName,
            designation: item.designation ?? "",
            bio: item.bio ?? "",
            sortOrder: item.sortOrder,
          }
        : { fullName: "", designation: "", bio: "", sortOrder: advocates.length },
    );
    setAdvocatePhoto(null);
    setAdvocateDialog({ open: true, item });
  };

  const saveAdvocate = async () => {
    let advocateId = advocateDialog.item?.id;
    if (advocateDialog.item) {
      await websiteService.updateAdvocate(advocateDialog.item.id, advocateForm);
    } else {
      const res = await websiteService.createAdvocate(advocateForm);
      advocateId = res.data.id;
    }
    if (advocatePhoto && advocateId) {
      await websiteService.uploadAdvocatePhoto(advocateId, advocatePhoto);
    }
    setAdvocateDialog({ open: false });
    await loadAll();
    setMessage("Advocate saved.");
  };

  return (
    <>
      <PageHeader
        title="Website Content"
        subtitle={
          firmCode
            ? `Manage your public landing page at /${firmCode}${user?.firmName ? ` · ${user.firmName}` : ""}`
            : undefined
        }
      />

      {message && (
        <Alert severity="success" sx={{ mb: 2 }} onClose={() => setMessage("")}>
          {message}
        </Alert>
      )}

      <Tabs
        value={tab}
        onChange={(_, v) => setTab(v)}
        variant="scrollable"
        scrollButtons="auto"
        allowScrollButtonsMobile
        sx={{ mb: 3 }}
      >
        <Tab label="General" />
        <Tab label="Banners" />
        <Tab label="Practice" />
        <Tab label="Advocates" />
      </Tabs>

      {tab === 0 && (
        <Card>
          <CardContent sx={{ p: { xs: 2, sm: 3 } }}>
            <Grid container spacing={2}>
              <Grid size={{ xs: 12, md: 6 }}>
                <TextField label="Address" fullWidth margin="dense" value={profile.addressLine} onChange={(e) => setProfile({ ...profile, addressLine: e.target.value })} />
                <TextField label="City / State" fullWidth margin="dense" value={profile.city} onChange={(e) => setProfile({ ...profile, city: e.target.value })} />
                <TextField label="Phone" fullWidth margin="dense" value={profile.phone} onChange={(e) => setProfile({ ...profile, phone: e.target.value })} />
                <TextField label="Email" fullWidth margin="dense" value={profile.email} onChange={(e) => setProfile({ ...profile, email: e.target.value })} />
              </Grid>
              <Grid size={{ xs: 12, md: 6 }}>
                <TextField label="Hero tagline" fullWidth margin="dense" value={profile.heroTagline} onChange={(e) => setProfile({ ...profile, heroTagline: e.target.value })} />
                <TextField label="Hero subtitle" fullWidth margin="dense" multiline rows={2} value={profile.heroSubtitle} onChange={(e) => setProfile({ ...profile, heroSubtitle: e.target.value })} />
              </Grid>
              <Grid size={12}>
                <TextField label="About title" fullWidth margin="dense" value={profile.aboutTitle} onChange={(e) => setProfile({ ...profile, aboutTitle: e.target.value })} />
                <TextField label="About body" fullWidth margin="dense" multiline rows={3} value={profile.aboutBody} onChange={(e) => setProfile({ ...profile, aboutBody: e.target.value })} />
                <TextField label="Highlight card title" fullWidth margin="dense" value={profile.aboutHighlightTitle} onChange={(e) => setProfile({ ...profile, aboutHighlightTitle: e.target.value })} />
                <TextField label="Highlight card body" fullWidth margin="dense" multiline rows={2} value={profile.aboutHighlightBody} onChange={(e) => setProfile({ ...profile, aboutHighlightBody: e.target.value })} />
              </Grid>
            </Grid>
            <Button variant="contained" sx={{ mt: 2, bgcolor: "#0d1b2a" }} onClick={saveProfile}>
              Save Profile
            </Button>
          </CardContent>
        </Card>
      )}

      {tab === 1 && (
        <Stack spacing={3}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Upload banner photo
              </Typography>
              <Stack direction={{ xs: "column", sm: "row" }} spacing={2} sx={{ alignItems: "center" }}>
                <Button variant="outlined" component="label">
                  Choose image
                  <input hidden type="file" accept="image/*" onChange={(e) => setBannerFile(e.target.files?.[0] ?? null)} />
                </Button>
                <TextField label="Caption (optional)" size="small" value={bannerCaption} onChange={(e) => setBannerCaption(e.target.value)} />
                <Button variant="contained" disabled={!bannerFile} onClick={uploadBanner}>
                  Upload
                </Button>
              </Stack>
              <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                Photos auto-scroll on your landing page hero section.
              </Typography>
            </CardContent>
          </Card>
          <Grid container spacing={2}>
            {banners.map((banner) => (
              <Grid key={banner.id} size={{ xs: 12, sm: 6, md: 4 }}>
                <Card>
                  <CardMedia component="img" height="160" image={publicMediaUrl(banner.imageUrl)} alt={banner.caption ?? "Banner"} />
                  <CardContent>
                    <Typography variant="body2">{banner.caption || "No caption"}</Typography>
                    <IconButton color="error" onClick={async () => { await websiteService.deleteBanner(banner.id); await loadAll(); }}>
                      <DeleteIcon />
                    </IconButton>
                  </CardContent>
                </Card>
              </Grid>
            ))}
          </Grid>
        </Stack>
      )}

      {tab === 2 && (
        <>
          <Box sx={{ mb: 2 }}>
            <Button variant="contained" onClick={() => openAreaDialog()}>Add Practice Area</Button>
          </Box>
          <Grid container spacing={2}>
            {practiceAreas.map((area) => (
              <Grid key={area.id} size={{ xs: 12, md: 6 }}>
                <Card>
                  <CardContent>
                    <Typography variant="h6">{area.title}</Typography>
                    <Typography color="text.secondary">{area.description}</Typography>
                    <Stack direction="row" sx={{ mt: 1 }}>
                      <IconButton onClick={() => openAreaDialog(area)}><EditIcon /></IconButton>
                      <IconButton color="error" onClick={async () => { await websiteService.deletePracticeArea(area.id); await loadAll(); }}>
                        <DeleteIcon />
                      </IconButton>
                    </Stack>
                  </CardContent>
                </Card>
              </Grid>
            ))}
          </Grid>
        </>
      )}

      {tab === 3 && (
        <>
          <Box sx={{ mb: 2 }}>
            <Button variant="contained" onClick={() => openAdvocateDialog()}>Add Advocate</Button>
          </Box>
          <Grid container spacing={2}>
            {advocates.map((advocate) => (
              <Grid key={advocate.id} size={{ xs: 12, md: 6 }}>
                <Card>
                  <CardContent sx={{ display: "flex", gap: 2 }}>
                    <Avatar src={advocate.photoUrl ? publicMediaUrl(advocate.photoUrl) : undefined} sx={{ width: 64, height: 64 }}>
                      {advocate.fullName.charAt(0)}
                    </Avatar>
                    <Box sx={{ flex: 1 }}>
                      <Typography variant="h6">{advocate.fullName}</Typography>
                      <Typography variant="body2" sx={{ color: "#c9a227" }}>{advocate.designation}</Typography>
                      <Typography variant="body2" color="text.secondary">{advocate.bio}</Typography>
                      <Stack direction="row" sx={{ mt: 1 }}>
                        <IconButton onClick={() => openAdvocateDialog(advocate)}><EditIcon /></IconButton>
                        <IconButton color="error" onClick={async () => { await websiteService.deleteAdvocate(advocate.id); await loadAll(); }}>
                          <DeleteIcon />
                        </IconButton>
                      </Stack>
                    </Box>
                  </CardContent>
                </Card>
              </Grid>
            ))}
          </Grid>
        </>
      )}

      <Dialog open={areaDialog.open} onClose={() => setAreaDialog({ open: false })} fullWidth maxWidth="sm">
        <DialogTitle>{areaDialog.item ? "Edit" : "Add"} Practice Area</DialogTitle>
        <DialogContent>
          <TextField label="Title" fullWidth margin="dense" value={areaForm.title} onChange={(e) => setAreaForm({ ...areaForm, title: e.target.value })} />
          <TextField label="Description" fullWidth margin="dense" multiline rows={3} value={areaForm.description} onChange={(e) => setAreaForm({ ...areaForm, description: e.target.value })} />
          <TextField label="Sort order" type="number" fullWidth margin="dense" value={areaForm.sortOrder} onChange={(e) => setAreaForm({ ...areaForm, sortOrder: Number(e.target.value) })} />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setAreaDialog({ open: false })}>Cancel</Button>
          <Button variant="contained" onClick={saveArea}>Save</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={advocateDialog.open} onClose={() => setAdvocateDialog({ open: false })} fullWidth maxWidth="sm">
        <DialogTitle>{advocateDialog.item ? "Edit" : "Add"} Advocate</DialogTitle>
        <DialogContent>
          <TextField label="Full name" fullWidth margin="dense" value={advocateForm.fullName} onChange={(e) => setAdvocateForm({ ...advocateForm, fullName: e.target.value })} />
          <TextField label="Designation" fullWidth margin="dense" value={advocateForm.designation} onChange={(e) => setAdvocateForm({ ...advocateForm, designation: e.target.value })} />
          <TextField label="Brief bio" fullWidth margin="dense" multiline rows={3} value={advocateForm.bio} onChange={(e) => setAdvocateForm({ ...advocateForm, bio: e.target.value })} />
          <TextField label="Sort order" type="number" fullWidth margin="dense" value={advocateForm.sortOrder} onChange={(e) => setAdvocateForm({ ...advocateForm, sortOrder: Number(e.target.value) })} />
          <Button variant="outlined" component="label" sx={{ mt: 2 }}>
            {advocatePhoto ? advocatePhoto.name : "Choose photo"}
            <input hidden type="file" accept="image/*" onChange={(e) => setAdvocatePhoto(e.target.files?.[0] ?? null)} />
          </Button>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setAdvocateDialog({ open: false })}>Cancel</Button>
          <Button variant="contained" onClick={saveAdvocate}>Save</Button>
        </DialogActions>
      </Dialog>
    </>
  );
};

export default WebsitePage;
