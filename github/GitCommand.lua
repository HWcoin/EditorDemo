
--[[
    1. 创建版本仓库
    切换到工作目录
    git init

    2. 查看工作目录状态
    git status

    3. 查看文件修改内容
    git diff readme.txt

    4. 提交修改,可分多次add,最后一次commit, git add . 添加全部
    git add readme.txt
    git commit [-m msg]

    git push

    5. 查看日志
    git log [--pretty=oneline]

    6. 版本回退 HEAD^^上上一个版本
    HEAD^或者版本号，版本号不必写全
    git reset --hard HEAD^

    7. checkout
    git checkout -- readme.txt

    8. 创建远程仓库
    git pull --rebase origin master
    git remote add origin git@github.com:michaelliao/ProjectName.git
    git push -u origin master

    9. 登录
    git config --global user.name "Your Name"
    git config --global user.email "email@example.com"

    10. update
    git pull origin master

    11. 从远程库clone
    git clone git@github.com:michaelliao/gitskills.git

    12. 修改远程仓库
    一、修改命令
    git remote set-url origin url
    二、先删后加
    git remote rm origin
    git remote add origin git@github.com:sheng/demo.git
    三、修改config文件

--]] 