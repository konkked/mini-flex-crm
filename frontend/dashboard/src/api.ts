import axios from "axios";
import { jwtDecode } from "jwt-decode";
import { Relation, Relationships } from "./models/relation";
import { Company } from "./models/company";
import { User } from "./models/user";
import { Customer } from "./models/customer";

const API_BASE_URL = process.env.API_BASE_URL || "http://localhost:8080/api";

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

export const getCurrentRole = () => getCurrentUser()?.role;
export const getCurrentUserId = () => getCurrentUser()?.sub;
export const hasAdminAccessToItem = (item: Customer | Company | User | Relation) => {
    return getCurrentRole() === "admin" 
            && (item.tenantId == getCurrentTenantId() || getCurrentTenantId() == "0");
}


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


const entities = ["user", "customer", "company", "relation"];

// API Object that will dynamically contain all API calls
const api: Record<string, any> = {};


export const getCurrentTenantId = () => getCurrentUser()?.tenant_id;
 
entities.forEach((entity) => {
  api.admin = {};
  api.admin[entity] = {
    list: {
        next: async (tenantId?: string, nextToken?: string, search?: any) => {
            const params = nextToken 
                ? search 
                    ? { next: nextToken, search: base62Encode(JSON.stringify(search)) } 
                    : { next: nextToken }
                : search 
                    ? { search: base62Encode(JSON.stringify(search)) } 
                    : {};
            const response = await apiClient.get(tenantId ? `/tenant/${tenantId}/${entity}` : `/${entity}`, { params });
            return response.data;
        },
        prev: async (tenantId?: string, prevToken?: string, search?: any) => {
            const params = prevToken 
                ? search 
                    ? { next: prevToken, search: base62Encode(JSON.stringify(search)) } 
                    : { next: prevToken }
                : search 
                    ? { search: base62Encode(JSON.stringify(search)) } 
                    : {};
            const response = await apiClient.get(tenantId ? `/tenant/${tenantId}/${entity}` : `/${entity}`, { params });
            return response.data;
        }
    },
    search: async (tenantId?: string, nextToken?: string, search?: any) => {
        const params = nextToken 
            ? search 
                ? { next: nextToken, search: base62Encode(JSON.stringify(search)) } 
                : { next: nextToken }
            : search 
                ? { search: base62Encode(JSON.stringify(search)) } 
                : {};
        const response = await apiClient.get(tenantId ? `/tenant/${tenantId}/${entity}` : `/${entity}`, { params });
        return response.data;
    },
    get: async (tenantId: string, id: number) => {
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
 

  api.std = {};
  api.std[entity] = {
    list: { 
        next: async (nextToken?: string) => await api.admin[entity].list(getCurrentTenantId(), nextToken), 
        prev: async (prevToken?: string) => await api.admin[entity].list(getCurrentTenantId(), prevToken) 
    },
    search: async (criteria: any, nextToken?: string) => await api.admin[entity].list(getCurrentTenantId(), nextToken, criteria),
    get: async (id: number) => await api.admin[entity].get(getCurrentTenantId(), id),
    create: async (data: any) => await api.admin[entity].create(getCurrentTenantId(), data),
    edit: async (id: number, data: any) => await api.admin[entity].edit(getCurrentTenantId(), id, data),
    delete: async (id: number) => await api.admin[entity].delete(getCurrentTenantId(), id)
  };

});

api.std.customer.get_with_relationships = async (tenantId: string, id: number) => {
    const response = await apiClient.get<Relationships>(`/tenant/${tenantId}/customer/${id}/relationships`);
    return response.data;
};

api.admin.tenant = {
    list: { 
        next : async (nextToken?: string) => {
            const params = nextToken ? { next: nextToken } : {};
            const response = await apiClient.get(`/tenant`, { params });
            return response.data;
        }, 
        prev : async (prevToken?: string) => {
            const params = prevToken ? { prev: prevToken } : {};
            const response = await apiClient.get(`/tenant`, { params });
            return response.data;
        }
    },
    get: async (tenantId: string, id: number) => {
        const response = await apiClient.get(`/tenant/${tenantId}/${id}`);
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

// enable and disable users need to be added.
api.admin.user.enable = async (tenantId: string, userId: number) => {
  return apiClient.post(`/tenant/${tenantId}/user/${userId}/enable`);
};

api.admin.user.disable = async (tenantId: string, userId: number) => {
  return apiClient.post(`/tenant/${tenantId}/user/${userId}/disable`);
};

// Authentication API Calls
api.auth = {
  login: async (username: string, password: string) => {
    const response = await apiClient.post("/auth/login", { username, password });
    if (response.data.token) setAuthToken(response.data.token);
    return response.data;
  },
  signup: async (username: string, password: string, tenantId: number) => {
    const response = await apiClient.post("/auth/signup", { username, password, tenantId });
    return response.data;
  },
  refreshToken: async (token: string) => {
    const response = await apiClient.post("/auth/refresh", { token });
    setAuthToken(response.data.token);
    return response.data;
  },
};

// Export the dynamic API
export default api;