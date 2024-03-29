﻿using AutoMapper;
using Icarus.DB.Entities.DataContext;
using Icarus.Model;
using Icarus.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icarus.Service.User
{
    public class UserService : IUserService
    {
        // mapper çağırılıyor
        private readonly IMapper mapper;
        public UserService(IMapper _mapper)
        {
            mapper = _mapper;
        }
        // Kullanıcı girişi işleminin gerçekleştirildiği metot
        public General<LoginViewModel> Login(LoginViewModel user)
        {
            var result = new General<LoginViewModel>();
            var model = mapper.Map<Icarus.DB.Entities.User>(user);

            using (var context = new IcarusContext())
            {
                // Kullanıcı adı, IsActive, IsDeleted ve şifre alanı kontrol ediliyor
                // gerekli şartlar sağlandıysa IsSuccess true sağlanmadıysa false dönüyor
                result.Entity = mapper.Map<LoginViewModel>(model);
                result.IsSuccess = context.User.Any(
                    x => x.UserName == user.UserName &&
                                       x.IsActive &&
                                       !x.IsDeleted &&
                                       x.Password == user.Password);
            }

            return result;
        }
        // Tüm kullanıcıların listelendiği kısım
        public General<UserViewModel> GetUsers()
        {
            var result = new General<UserViewModel>();

            using (var context = new IcarusContext())
            {
                // IsActive true; IsDeleted false ise id'ye göre listeleme gerçekleştiriliyor
                var data = context.User
                    .Where(x => x.IsActive && !x.IsDeleted)
                    .OrderBy(x => x.Id);
                // veritabanında kullanıcı varsa kullanılar listeleniyor yoksa mesaj dönüyor
                if (data.Any())
                {
                    result.List = mapper.Map<List<UserViewModel>>(data);
                    result.IsSuccess = true;
                }
                else
                {
                    result.ExceptionMessage = "Hiçbir kullanıcı bulunamadı.";
                }
            }

            return result;
        }
        // Kullanıcı ekleme işleminin gerçekleştirildiği metot
        public General<UserViewModel> Insert(UserViewModel newUser)
        {
            var result = new General<UserViewModel>();
            var model = mapper.Map<Icarus.DB.Entities.User>(newUser);

            using (var context = new IcarusContext())
            {
                model.Idate = DateTime.Now;
                context.User.Add(model);
                context.SaveChanges();

                result.Entity = mapper.Map<UserViewModel>(model);
                result.IsSuccess = true;
            }

            return result;
        }
        // Kullanıcı güncelleme işleminin gerçekleştirildiği metot
        public General<UserViewModel> Update(int id, UserViewModel user)
        {
            var result = new General<UserViewModel>();

            using (var context = new IcarusContext())
            {
                // Gelen modeldeki kullanıcı veritabanında bulunuyor mu kontrol ediliyor
                var updateUser = context.User.SingleOrDefault(i => i.Id == id);

                // Kullanıcı varsa güncelleniyor yoksa mesaj dönüyor
                if (updateUser is not null)
                {
                    updateUser.Name = user.Name;
                    updateUser.Surname = user.Surname;
                    updateUser.UserName = user.UserName;
                    updateUser.Email = user.Email;
                    updateUser.Password = user.Password;

                    context.SaveChanges();

                    result.Entity = mapper.Map<UserViewModel>(updateUser);
                    result.IsSuccess = true;
                }
                else
                {
                    result.ExceptionMessage = "Kullanıcı bulunamadı.";
                }
            }

            return result;
        }
        // Kullanıcı silme işleminin gerçekleştirildiği metot
        public General<UserViewModel> Delete(int id)
        {
            var result = new General<UserViewModel>();

            using (var context = new IcarusContext())
            {
                // parametredeki id'ye ait kullanıcı var mı kontrol ediliyor
                var user = context.User.SingleOrDefault(i => i.Id == id);

                // Kullanıcı varsa siliniyor yoksa mesaj dönüyor
                if (user is not null)
                {
                    context.User.Remove(user);
                    context.SaveChanges();

                    result.Entity = mapper.Map<UserViewModel>(user);
                    result.IsSuccess = true;
                }
                else
                {
                    result.ExceptionMessage = "Kullanıcı bulunamadı.";
                }
            }

            return result;
        }
    }
}
