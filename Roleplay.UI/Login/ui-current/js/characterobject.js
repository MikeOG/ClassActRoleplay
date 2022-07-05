class Character {
    constructor(charData) {
        this.data = charData;
    }

    get FirstName() {
        return this.data.FirstName;
    }

    get LastName() {
        return this.data.LastName;
    }

    get FullName() {
        return `${this.FirstName} ${this.LastName}`;
    }

    get JailTime() {
        return this.data.JailTime;
    }

    get BirthDate() {
        return this.data.DOB;
    }

    get CharID() {
        return this.data.CharID;
    }

    get Job() {
        return this.data.Job;
    }

    get IsEnabled() {
        return this.data.IsActive;
    }

    createCardContent() {
        var baseElem = document.createElement("div");
        baseElem.classList.add("card-content");
        baseElem.classList.add("white-text");

        var spanElem = document.createElement("span");
        spanElem.classList.add("card-title");
        spanElem.innerHTML = this.FullName;
        baseElem.appendChild(spanElem);

        var dob = document.createElement("p");
        dob.innerHTML = `<b>DOB: </b>${this.BirthDate}`;
        baseElem.appendChild(dob);

        var job = document.createElement("p");
        job.innerHTML = `<b>Occupation: </b>${this.Job}`;
        baseElem.appendChild(job);

        var jail = document.createElement("p");
        jail.innerHTML = `<b>Jail time: </b>${this.JailTime}`;
        baseElem.appendChild(jail);

        return baseElem;
    }

    /*createCardAction() {
        var baseElem = document.createElement("div");
        baseElem.innerHTML = `<div class="card-action">
                                <div class="row">
                                    <a class="col s5 btn waves-effect waves-light z-depth-2 hoverable right" id="character-select" data-charid="${this.CharID}">Select character
                                        <i class="material-icons right">send</i>
                                    </a>
                                    <a class="col s5 btn waves-effect waves-light z-depth-2 hoverable disabled activator" id="character-delete">Delete character
                                        <i class="material-icons right">send</i>
                                    </a>
                                </div>
                            </div>`;

        return baseElem;
    }

    createCardReveal() {
        var baseElem = document.createElement("div");
        baseElem.innerHTML = `<div class="card-reveal">
                                <span class="card-title grey-text text-darken-4">Are you sure?<i class="material-icons right">close</i></span>
                                <p>Deleting a character is an irreverisble action and cannot be undone</p>
                                <a class="col s5 btn waves-effect waves-light z-depth-2 hoverable" data-charid="${this.CharID}">Delete character
                                    <i class="material-icons right">send</i>
                                </a>
                                <div class="row">
                                    <div class="col s12 progress scale-transition scale-out" id="login-progess" >
                                        <div class="indeterminate"></div>
                                    </div>
                                </div>
                            </div>`
        return baseElem;
    }

    getWholeCard(){
        var baseElem = document.createElement("div");
        baseElem.classList.add("card");
        baseElem.classList.add("grey");
        baseElem.classList.add("darken-1");
        baseElem.classList.add("z-depth-1");

        var content = this.createCardContent();
        var action = this.createCardAction();
        var reveal = this.createCardReveal();

        baseElem.appendChild(content);
        baseElem.appendChild(action);
        baseElem.appendChild(reveal);

        return baseElem;
    }*/

    CreateCharacterCard(){
        /*var baseElem = document.createElement("div");
        baseElem.classList.add("col");
        baseElem.classList.add("s3");

        baseElem.appendChild(this.getWholeCard());*/
        // lol have to do this due to DOM (or something else idk)
        var baseElem = document.createElement("div");
        var disableClass = " ";
        if(!this.IsEnabled)
            disableClass = " disabled ";
        baseElem.innerHTML = `<div class="col s3">
                                <div class="card grey darken-1 z-depth-2">
                                    <div class="card-content white-text">
                                        <span class="card-title">${this.FullName}</span>
                                        <p><b>DOB: </b>${this.BirthDate}</p>
                                        <p><b>Occupation: </b>${this.Job}</p>
                                        <p><b>Jail time: </b>${this.JailTime} months</p>
                                    </div>
                                    <div class="card-action">
                                        <div class="row">
                                            <a onclick="OnDoLogin(this)" class="col s5 btn waves-effect waves-light z-depth-2 hoverable${disableClass}right" id="btn-select-char" unique-iden="select-${this.CharID}" data-charid="${this.CharID}">Select character
                                                <i class="material-icons right">send</i>
                                            </a>
                                            <a class="col s5 btn waves-effect waves-light z-depth-2 hoverable disabled activator" id="btn-delete-char" unique-iden="delete-${this.CharID}">Delete character
                                                <i class="material-icons right">send</i>
                                            </a>
                                        </div>
                                    </div>
                                    <div class="card-reveal">
                                        <span class="card-title grey-text text-darken-4">Are you sure?<i class="material-icons right">close</i></span>
                                        <p>Deleting a character is an irreverisble action and cannot be undone</p>
                                        <a onclick="OnCharacterDelete(${this.CharID})" class="col s5 btn waves-effect waves-light z-depth-2 hoverable activator" data-charid="${this.CharID}">Delete character
                                            <i class="material-icons right">send</i>
                                        </a>
                                        <div class="row">
                                            <div class="col s12 progress scale-transition scale-out" id="login-progess" >
                                                <div class="indeterminate"></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>`;

        return baseElem;
    }
}