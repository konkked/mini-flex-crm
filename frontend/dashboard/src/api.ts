import axios from "axios";
import { jwtDecode } from "jwt-decode";
import { Relationship, PivotedRelationships } from "./models/relationship";
import { Company } from "./models/company";
import { User } from "./models/user";
import { Account } from "./models/account";
import { Note } from "./models/note";
import { Team } from "models/team";

const API_BASE_URL = process.env.API_BASE_URL || "http://localhost:5111/api";

// Function to base62 encode a string
export const base62Encode = (str: string): string => {
  const chars = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
  let encoded = '';
  let num = BigInt('0x' + Array.from(str).map(c => c.charCodeAt(0).toString(16)).join(''));
  while (num > 0) {
    encoded = chars[Number(num % 62n)] + encoded;
    num = num / 62n;
  }
  return encoded;
};

// Create Axios instance
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: { "Content-Type": "application/json" },
});

// Define the expected structure of the JWT payload
interface JwtPayload {
    sub: string; // User ID
    unique_name: string; // Username
    tenant_id: string; // Tenant ID
    tenant_name: string; // Tenant Name
    logo: string; // Tenant Logo
    theme?: string; // User Theme
    role: string; // User role
    exp?: number; // Expiration timestamp (optional)
}
  
// Function to decode JWT token and extract claims
export const extractClaims = (token: string | null): JwtPayload | null => {
    if (!token) return null;
    try {
        const decoded = jwtDecode<JwtPayload>(token);
        return decoded;
    } catch (error) {
        console.error("Invalid token:", error);
        return null;
    }
};

export const getCurrentUser = () => {
    const token = localStorage.getItem("token");
    if (!token || isTokenExpired(token)) return null;
    return extractClaims(token);
};

export const getCurrentTheme = () => getCurrentUser()?.theme || 'professional';
export const getCurrentRole = () => getCurrentUser()?.role;
export const getCurrentUserId = () => getCurrentUser()?.sub;
export const hasAdminAccessToItem = (item: Account | Company | User | Relationship) => {
    return getCurrentRole() === "admin" 
            && (item.tenantId == getCurrentTenantId() || getCurrentTenantId() == 0);
}
export const hasAccessToEditTeam=(team: Team)=>{
  return getCurrentRole() === "admin" || getCurrentRole() === "manager"
          && (team.tenantId == getCurrentTenantId() || getCurrentTenantId() == 0);
}


export const isSuperAdmin = () => getCurrentRole() == "admin" && getCurrentTenantId() == 0;

// Function to set token in headers
export const setAuthToken = (token: string | null) => {
  if (token) {
    apiClient.defaults.headers.common["Authorization"] = `Bearer ${token}`;
    localStorage.setItem("token", token);
  } else {
    delete apiClient.defaults.headers.common["Authorization"];
    localStorage.removeItem("token");
  }
};

// Function to get token from localStorage
const getAuthToken = (): string | null => {
  return localStorage.getItem("token");
};

// Function to check if token is expired
const isTokenExpired = (token: string) => {
  try {
    const decoded: any = jwtDecode(token);
    return decoded.exp * 1000 < Date.now();
  } catch (err) {
    console.log('Token is invalid or expired', err);
    return true;
  }
};

// Function to refresh token
const refreshAuthToken = async () => {
  const token = localStorage.getItem("token");
  if (!token) return null;

  try {
    const response = await apiClient.post("/auth/refresh", { token });
    const newToken = response.data.token;
    setAuthToken(newToken);
    return newToken;
  } catch (error) {
    console.error("Token refresh failed", error);
    setAuthToken(null);
    return null;
  }
};

// Interceptor to refresh token before requests
apiClient.interceptors.request.use(async (config) => {
  let token = getAuthToken();
  if (token && isTokenExpired(token)) {
    token = await refreshAuthToken();
  }
  
  if (token) {
    config.headers["Authorization"] = `Bearer ${token}`;
  }

  return config;
});

const entities = ["user", "customer", "company", "relationship", "contact", "account", "address", "interaction", "lead", "payment", "product", "sale", "deal", "support-ticket", "team"];

// API Object that will dynamically contain all API calls
const api: Record<string, any> = {};

export const getCurrentTenantId = () => Number.parseInt(getCurrentUser()?.tenant_id ?? '-1');
export const getCurrentTenantFromToken = () => {
  let user = getCurrentUser();
  return { name: user?.tenant_name, logo: user?.logo, id: user?.tenant_id };
};

entities.forEach((entity) => {
  api.admin ??= {};
  const fetch = async (tenantId?: number, offset?: number, limit?: number, search?: any) => {
    const params : Record<string, any> = {};
    params.offset = offset ?? 0;
    params.limit = limit ?? 50;
    if (search) {
        params.search = base62Encode(JSON.stringify(search));
    }
    const response = await apiClient.get(tenantId !== -1 ? `/tenant/${tenantId}/${entity}` : `/${entity}`, { params });
    return response.data;
  }
  api.admin[entity] = {
    list: async (tenantId?:number, offset?: number, limit?: number) => await fetch(tenantId, offset, limit),
    search: async (tenantId?:number, offset?: number, limit?: number, query?:any) => await fetch(tenantId, offset, limit, query),
    get: async (tenantId: string, id: number) => {
      if(!id){
        id = Number(tenantId);
        tenantId = getCurrentTenantId().toString();
      }
      const response = await apiClient.get(`/tenant/${tenantId}/${entity}/${id}`);
      return response.data;
    },
    create: async (tenantId: string, data: any) => {
      const response = await apiClient.post(`/tenant/${tenantId}/${entity}`, data);
      return response.data;
    },
    edit: async (tenantId: string, id: number, data: any) => {
      const response = await apiClient.put(`/tenant/${tenantId}/${entity}/${id}`, data);
      return response.data;
    },
    delete: async (tenantId: string, id: number) => {
      const response = await apiClient.delete(`/tenant/${tenantId}/${entity}/${id}`);
      return response.data;
    },
  };

  api.std ??= {};
  api.std[entity] = {
    list: async (offset?: number, limit?: number) => await api.admin[entity].list(getCurrentTenantId(), offset, limit),
    search: async (offset?: number, limit?: number, query?: any) => await api.admin[entity].search(getCurrentTenantId(), offset, limit, query),
    get: async (id: number) => await api.admin[entity].get(getCurrentTenantId().toString(), id),
    create: async (data: any) => await api.admin[entity].create(getCurrentTenantId().toString(), data),
    edit: async (id: number, data: any) => await api.admin[entity].edit(getCurrentTenantId().toString(), id, data),
    delete: async (id: number) => await api.admin[entity].delete(getCurrentTenantId().toString(), id)
  };
});

// Add specific methods for account relationships
api.std.account.getRelationships = async (id: number) => {
  const response = await apiClient.get<PivotedRelationships>(`/tenant/${getCurrentTenantId()}/account/${id}/relationships`);
  return response.data;
};

api.std.team.getPotentialMembers = async (data: { teamId: number, name: string }) => {
  const response = await apiClient.get(`/tenant/${getCurrentTenantId()}/team/${data.teamId}/potential_members`, { params: { name: data.name } });
  return response.data;
}

api.std.team.getPotentialOwners = async (data: { teamId: number, name: string }) => {
  const response = await apiClient.get(`/tenant/${getCurrentTenantId()}/team/${data.teamId}/potential_owners`, { params: { name: data.name } });
  return response.data;
}

api.std.team.mine = async () => {
  const response = await apiClient.get(`/tenant/${getCurrentTenantId()}/team/mine`);
  return response.data;
}

api.std.lead.pipeline = api.std.lead.get;
api.std.deal.pipeline = api.std.deal.get;

// Add tenant-specific methods
api.admin.tenant = {
  list: async (offset?: number, limit?: number) : Promise<any[]> => {
      const params = { offset: offset ?? 0, limit: limit ?? 50 };
      const response = await apiClient.get(`/tenant`, { params });
      return response.data;
  },
  searchByName: async (name: string) : Promise<any[]> => {
      const params = { offset: 0, limit: 1000, search: base62Encode(JSON.stringify({name})) };
      const response = await apiClient.get(`/tenant`, { params });
      return response.data;
  },
  get: async (id: number) => {
      const response = await apiClient.get(`/tenant/${id}`);
      return response.data;
  },
  create: async (data: any) => {
      const response = await apiClient.post(`/tenant/`, data);
      return response.data;
  },
  edit: async (id: number, data: any) => {
      const response = await apiClient.put(`/tenant/${id}`, data);
      return response.data;
  },
  delete: async (id: number) => {
      const response = await apiClient.delete(`/tenant/${id}`);
      return response.data;
  }
};

const getCurrentTenant = async () => {
  const response = await apiClient.get(`/tenant/${getCurrentTenantId()}/mine`);
  return response.data;
}

api.std.tenant.mine = getCurrentTenant;

// Enable and disable users
api.admin.user.enable = async (userId: number) => await apiClient.post(`/tenant/${getCurrentTenantId()}/user/${userId}/enable`);
api.admin.user.disable = async (userId: number) => await apiClient.post(`/tenant/${getCurrentTenantId()}/user/${userId}/disable`);
api.admin.user.edit = async (userId: number, data: any) => await apiClient.put(`/tenant/${data.tenantId}/user/${userId}`, data);

// Profile image endpoints
api.std.user.getProfileImage = async (userId: number) => {
  const response = await apiClient.get(`/tenant/${getCurrentTenantId()}/user/${userId}/profile-image`);
  return response.data;
};

api.std.user.uploadProfileImage = async (userId: number, imageData: string) => {
  const response = await apiClient.post(`/tenant/${getCurrentTenantId()}/user/${userId}/profile-image`, { image: imageData });
  return response.data;
};

// Authentication API Calls
api.auth = {
  login: async (username: string, password: string) => {
    console.log('inside of login');
    setAuthToken(null);
    const response = await apiClient.post("/auth/login", { username, password });
    console.log('response:', response);
    if (response.data.token) setAuthToken(response.data.token);
    return getCurrentUser();
  },
  signup: async (username: string, name: string, email: string, password: string, tenantId: number) => {
    const response = await apiClient.post("/auth/signup", { username, name, email, password, tenantId });
    return response.data;
  },
  refreshToken: async (token: string) => {
    const response = await apiClient.post("/auth/refresh", { token });
    setAuthToken(response.data.token);
    return response.data;
  },
};

// Notes API Calls
export const getNotes = async (route: string) : Promise<Note[]> => {
  const response = await apiClient.get(`/tenant/${getCurrentTenantId()}/notes/${route}`);
  response.data.forEach((e: { route?: string; }) => {
    e.route = route;
  });
  return response.data;
};

export const createNote = async (note: Note) : Promise<Number> => {
  const response = await apiClient.post(`/tenant/${getCurrentTenantId()}/notes/${note.route}`, note);
  return response.data;
};

export const updateNote = async (note: Note) : Promise<Number> => {
  const response = await apiClient.put(`/tenant/${getCurrentTenantId()}/notes/${note.route}`, note);
  return response.data;
};

export const deleteNote = async (id: number) => {
  await apiClient.delete(`/tenant/${getCurrentTenantId()}/notes/${id}`);
};

export const togglePinNote = async (id: number) => {
  await apiClient.put(`/tenant/${getCurrentTenantId()}/notes/${id}/pin`);
};

api.std.notes = {
  list: getNotes,
  create: createNote,
  update: updateNote,
  delete: deleteNote,
  togglePin: togglePinNote
}

// Export the dynamic API
export default api;