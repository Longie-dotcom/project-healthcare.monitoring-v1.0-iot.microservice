using Domain.Aggregate;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed
{
    public static class RolePrivilegeSeeder
    {
        public static async Task SeedAsync(IAMDBContext context)
        {
            // -----------------------------
            // Early exit if ADMIN role already exists
            // -----------------------------
            if (await context.Roles.AnyAsync(r => r.Code == "ADMIN"))
            { 
                Console.WriteLine("SEEDING EXITING, DATABASE HAS BEEN SEEDED.");
                return;
            }

            // -----------------------------
            // 1️ Seed Roles
            // -----------------------------
            var rolesToSeed = new List<Role>
            {
                new Role(Guid.NewGuid(), "Administrator", "ADMIN", "Has full access to all system features and management functions."),
                new Role(Guid.NewGuid(), "Doctor", "DOCTOR", "Responsible for diagnosing patients, creating treatment plans, and managing medical records."),
                new Role(Guid.NewGuid(), "Nurse", "NURSE", "Assists doctors in patient care, monitors treatments, and helps manage clinical operations."),
                new Role(Guid.NewGuid(), "Patient Family", "PATIENT_FAMILY", "Authorized family member who can view the patient’s appointments and test results."),
            };

            foreach (var role in rolesToSeed)
            {
                if (!await context.Roles.AnyAsync(r => r.Code == role.Code))
                    await context.Roles.AddAsync(role);
            }

            await context.SaveChangesAsync();

            // -----------------------------
            // 2️ Seed Privileges
            // -----------------------------
            var privilegeList = new (string Name, string Description)[]
            {
                // System & User Management
                ("ReadOnly", "Only have right to view patient test orders and results."),
                ("ViewUser", "Have right to view all user profiles."),
                ("CreateUser", "Have right to create new users."),
                ("ModifyUser", "Have right to modify user profiles."),
                ("DeleteUser", "Have right to delete users."),
                ("LockUnlockUser", "Have right to lock or unlock user accounts."),
                ("ViewRole", "Have right to view all roles and privileges."),
                ("CreateRole", "Have right to create new custom roles."),
                ("UpdateRole", "Have right to update role privileges."),
                ("DeleteRole", "Have right to delete custom roles."),
                ("ChangePassword", "Have right to change owned account password."),
                ("ViewPrivilege", "Have right to view all system privileges."),
                ("CreatePrivilege", "Have right to create new privileges."),
                ("UpdatePrivilege", "Have right to update existing privileges."),
                ("DeletePrivilege", "Have right to delete privileges."),
                ("ViewDevice", "Have right to view all devices."),
                ("CreateDevice", "Have right to create new custom device."),
                ("UpdateDevice", "Have right to update existed device."),
                ("DeleteDevice", "Have right to delete existed device."),

                // Patient Management
                ("ViewPatient", "Have right to view patient information and records."),
                ("CreatePatient", "Have right to create new patient records."),
                ("UpdatePatient", "Have right to update existing patient details."),
                ("DeletePatient", "Have right to delete patient records."),
                ("AssignPatientBed", "Have right to assign patients to available beds."),
                ("ReleasePatientBed", "Have right to release a bed assigned to a patient."),
                ("AssignPatientStaff", "Have right to assign staff members to a patient."),
                ("UnassignPatientStaff", "Have right to remove assigned staff from a patient."),

                // Patient Status Management
                ("ViewPatientStatus", "Have right to view all patient statuses."),
                ("CreatePatientStatus", "Have right to create new patient statuses."),
                ("UpdatePatientStatus", "Have right to update existing patient statuses."),
                ("DeletePatientStatus", "Have right to delete patient statuses."),

                // Staff Management
                ("ViewStaff", "Have right to view staff profiles and details."),
                ("CreateStaff", "Have right to create new staff records."),
                ("UpdateStaff", "Have right to update staff information."),
                ("DeleteStaff", "Have right to delete staff records.")
            };

            foreach (var (name, desc) in privilegeList)
            {
                if (!await context.Privileges.AnyAsync(p => p.Name == name))
                {
                    await context.Privileges.AddAsync(new Privilege(Guid.NewGuid(), name, desc));
                }
            }
            await context.SaveChangesAsync();

            // -----------------------------
            // 3️⃣ Seed Role-Privilege mappings
            // -----------------------------
            var rolesFromDb = await context.Roles.ToListAsync();
            var privilegesFromDb = await context.Privileges.ToListAsync();

            var rolePrivilegesToInsert = new List<RolePrivilege>();

            void AddPrivilegesToRole(string roleCode, params string[] privilegeNames)
            {
                var role = rolesFromDb.FirstOrDefault(r => r.Code == roleCode);
                if (role == null)
                    throw new Exception($"Role {roleCode} not found");

                foreach (var name in privilegeNames)
                {
                    var privilege = privilegesFromDb.FirstOrDefault(p => p.Name == name);
                    if (privilege == null)
                        throw new Exception($"Privilege {name} not found");

                    if (!rolePrivilegesToInsert.Any(rp => rp.RoleID == role.RoleID && rp.PrivilegeID == privilege.PrivilegeID))
                    {
                        rolePrivilegesToInsert.Add(new RolePrivilege(Guid.NewGuid(), role.RoleID, privilege.PrivilegeID, true));
                    }
                }
            }

            // ADMIN — Full access
            AddPrivilegesToRole("ADMIN", privilegesFromDb.Select(p => p.Name).ToArray());

            // DOCTOR — Core patient & staff operations
            AddPrivilegesToRole("DOCTOR",
                "ReadOnly", "ChangePassword",
                "ViewPatient", "CreatePatient", "UpdatePatient", "AssignPatientBed", "ReleasePatientBed",
                "ViewPatientStatus",
                "ViewStaff", "AssignPatientStaff", "UnassignPatientStaff"
            );

            // NURSE — Patient management assistance
            AddPrivilegesToRole("NURSE",
                "ReadOnly", "ChangePassword",
                "ViewPatient", "UpdatePatient",
                "AssignPatientBed", "ReleasePatientBed",
                "ViewPatientStatus"
            );

            // PATIENT_FAMILY — View-only permissions
            AddPrivilegesToRole("PATIENT_FAMILY",
                "ReadOnly", "ChangePassword",
                "ViewPatient", "ViewPatientStatus"
            );

            await context.RolePrivileges.AddRangeAsync(rolePrivilegesToInsert);
            await context.SaveChangesAsync();

            // -----------------------------
            // 4️ Seed Default Admin User
            // -----------------------------
            var adminEmail = "longdong32120@gmail.com";
            if (!await context.Users.AnyAsync(u => u.Email == adminEmail))
            {
                var adminRole = rolesFromDb.First(r => r.Code == "ADMIN");

                var adminUser = new User(
                    userID: Guid.NewGuid(),
                    email: adminEmail,
                    fullName: "Dong Xuan Bao Long",
                    dob: new DateTime(2005, 1, 28),
                    address: "Hiep Phuoc, Nhon Trach, Dong Nai",
                    gender: "Male",
                    phone: "+84349331141",
                    password: "28012005", // stored as plain for seeding
                    identityNumber: "077205011495",
                    isActive: true
                );

                adminUser.AddRole(adminRole.RoleID);

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
