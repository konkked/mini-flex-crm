import { Attributable } from "./attributable";
import { User } from "./user";
import { Account } from "./account";

export interface Team extends Attributable {
    id?: number;
    tenant?: string;
    tenantId?: number;
    name?: string;
    description?: string;
    managerId?: number;
    owner: TeamMemberUser;
    members?: TeamMember[];
    accounts?: Account[];
}

export interface TeamMember {
    role: string;
    user: TeamMemberUser;
}

export interface TeamMemberUser {
    id: number;
    name?: string;
    email?: string;
}